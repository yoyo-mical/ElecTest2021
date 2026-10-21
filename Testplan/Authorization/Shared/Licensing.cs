using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedAuthorization {
    public static class Products {
        public const string Timing = "SimTimingTools", Electrical = "ElecTest2026";
        public static void Require(string product) {
            if (product != Timing && product != Electrical) throw new InvalidDataException("未知产品。");
        }
    }
    [DataContract] public sealed class Grant {
        [DataMember] public string account;
        [DataMember] public string edition;
        [DataMember] public bool enabled;
        [DataMember] public string expiresUtc;
    }
    [DataContract] public sealed class LicenseList {
        [DataMember] public string product;
        [DataMember] public string issuedUtc;
        [DataMember] public List<Grant> users;
    }
    [DataContract] public sealed class SignedLicense {
        [DataMember] public string payload;
        [DataMember] public string signature;
    }
    public static class JsonCodec {
        public static string Serialize<T>(T value) {
            using (var stream = new MemoryStream()) {
                new DataContractJsonSerializer(typeof(T)).WriteObject(stream, value);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        public static T Deserialize<T>(string json) {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json.TrimStart('\ufeff'))))
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
        }
    }
    public static class Licensing {
        public const string ModuleVersion = "1.0.1";
        public static string CurrentAccount() { using (var identity = WindowsIdentity.GetCurrent()) return identity.Name; }
        public static string NormalizeEdition(string edition) {
            if (edition == "Beta" || edition == "Basic") return "Beta";
            if (edition == "Standard" || edition == "Pro") return "Standard";
            throw new InvalidDataException("授权版本无效。");
        }
        public static DateTimeOffset ParseUtc(string value) {
            DateTimeOffset parsed;
            if (!DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out parsed))
                throw new InvalidDataException("授权时间无效。");
            return parsed.ToUniversalTime();
        }
        public static LicenseList Verify(string json, string expectedProduct) {
            Products.Require(expectedProduct);
            var envelope = JsonCodec.Deserialize<SignedLicense>(json);
            if (envelope == null || string.IsNullOrWhiteSpace(envelope.payload) || string.IsNullOrWhiteSpace(envelope.signature))
                throw new InvalidDataException("缺少签名授权。");
            byte[] bytes = Convert.FromBase64String(envelope.payload), signature = Convert.FromBase64String(envelope.signature);
            using (var rsa = new RSACryptoServiceProvider()) {
                rsa.PersistKeyInCsp = false;
                rsa.FromXmlString(IssuerPublicKey.Xml);
                if (bytes.Length == 0 || !rsa.VerifyData(bytes, CryptoConfig.MapNameToOID("SHA256"), signature))
                    throw new UnauthorizedAccessException("授权签名无效。");
            }
            var list = JsonCodec.Deserialize<LicenseList>(new UTF8Encoding(false, true).GetString(bytes));
            if (list == null || list.product != expectedProduct || list.users == null)
                throw new UnauthorizedAccessException("授权名单不属于当前产品。");
            if (expectedProduct == Products.Electrical && string.IsNullOrWhiteSpace(list.issuedUtc))
                throw new InvalidDataException("ElecTest2026 名单缺少签发时间。");
            if (!string.IsNullOrWhiteSpace(list.issuedUtc)) ParseUtc(list.issuedUtc);
            return list;
        }
        internal static Grant RequireGrant(LicenseList list, string account) {
            var matches = list.users.Where(g => g != null && string.Equals(g.account, account, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (matches.Length != 1 || !matches[0].enabled) throw new UnauthorizedAccessException("账号未授权或已停用。");
            var grant = matches[0];
            NormalizeEdition(grant.edition);
            if (!string.IsNullOrWhiteSpace(grant.expiresUtc) && DateTimeOffset.UtcNow >= ParseUtc(grant.expiresUtc))
                throw new UnauthorizedAccessException("授权已到期。");
            return grant;
        }
    }
    public sealed class AuthorizationSession {
        private readonly string proof, product, account;
        private readonly DateTimeOffset refreshBy;
        private int revoked;
        public string Edition { get; private set; }
        public string Product { get { return product; } }
        private AuthorizationSession(string json, string expectedProduct) {
            product = expectedProduct; account = Licensing.CurrentAccount();
            var list = Licensing.Verify(json, product);
            AuthorizationHistory.Observe(list, account);
            var grant = Licensing.RequireGrant(list, account);
            proof = json; Edition = Licensing.NormalizeEdition(grant.edition);
            refreshBy = DateTimeOffset.UtcNow.AddHours(24);
        }
        public static AuthorizationSession Issue(string json, string expectedProduct) { return new AuthorizationSession(json, expectedProduct); }
        public void Ensure() {
            if (Volatile.Read(ref revoked) != 0 || DateTimeOffset.UtcNow >= refreshBy) throw new UnauthorizedAccessException("授权会话已撤销或需要刷新。");
            if (!string.Equals(account, Licensing.CurrentAccount(), StringComparison.OrdinalIgnoreCase)) throw new UnauthorizedAccessException("Windows 账号已变化。");
            var grant = Licensing.RequireGrant(Licensing.Verify(proof, product), account);
            if (Licensing.NormalizeEdition(grant.edition) != Edition) throw new UnauthorizedAccessException("会话与签名不一致。");
        }
        public void Revoke() { Interlocked.Exchange(ref revoked, 1); }
    }
    // Own one gate per product. Serialize refreshes; a timeout can never install a late result.
    public sealed class AuthorizationGate : IDisposable {
        private readonly string product;
        private readonly SemaphoreSlim refreshLock = new SemaphoreSlim(1, 1);
        private AuthorizationSession current;
        private int disposed;
        public AuthorizationGate(string expectedProduct) { Products.Require(expectedProduct); product = expectedProduct; }
        private AuthorizationSession RequireSession() {
            if (Volatile.Read(ref disposed) != 0) throw new ObjectDisposedException("AuthorizationGate");
            var session = Volatile.Read(ref current);
            if (session == null) throw new UnauthorizedAccessException("尚未通过启动授权校验。");
            session.Ensure();
            return session;
        }
        public void Ensure() { RequireSession(); }
        public string Edition { get { return RequireSession().Edition; } }
        private static void ObserveLateFault(Task read) { read.ContinueWith(t => { var ignored = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted); }
        public async Task RefreshAsync(string path) {
            await refreshLock.WaitAsync().ConfigureAwait(false);
            try {
                if (Volatile.Read(ref disposed) != 0) throw new ObjectDisposedException("AuthorizationGate");
                var read = Task.Run(() => File.ReadAllText(path, Encoding.UTF8));
                // Observe late faults without accepting a late read after timeout.
                ObserveLateFault(read);
                if (await Task.WhenAny(read, Task.Delay(10000)).ConfigureAwait(false) != read) throw new IOException("授权文件读取超时。");
                var next = AuthorizationSession.Issue(await read.ConfigureAwait(false), product);
                if (Volatile.Read(ref disposed) != 0) { next.Revoke(); throw new ObjectDisposedException("AuthorizationGate"); }
                var previous = Interlocked.Exchange(ref current, next);
                if (previous != null) previous.Revoke();
                if (Volatile.Read(ref disposed) != 0) Revoke();
            } catch { Revoke(); throw; }
            finally { refreshLock.Release(); }
        }
        public void Revoke() { var old = Interlocked.Exchange(ref current, null); if (old != null) old.Revoke(); }
        public void Dispose() { Interlocked.Exchange(ref disposed, 1); Revoke(); }
    }
}
