using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SharedAuthorization {
    // Local rollback resistance only: it cannot discover an unseen remote revocation.
    internal static class AuthorizationHistory {
        private static readonly object Gate = new object();
        private static byte[] Protect(byte[] value) { return ProtectedData.Protect(value, null, DataProtectionScope.CurrentUser); }
        private static byte[] Unprotect(byte[] value) { return ProtectedData.Unprotect(value, null, DataProtectionScope.CurrentUser); }
        private static string StoreDirectory(string product) { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), product, "AuthorizationHistory"); }
        internal static long IssuedTicks(string value) {
            if (string.IsNullOrWhiteSpace(value)) return 0; // legacy lists remain readable
            DateTimeOffset issued;
            if (!DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out issued))
                throw new UnauthorizedAccessException("授权名单签发时间无效。");
            return issued.UtcDateTime.Ticks;
        }
        internal static long RequireCurrent(long previous, long candidate) {
            if (candidate < previous) throw new UnauthorizedAccessException("该授权名单早于本机已检查的版本，请使用最新名单。");
            return Math.Max(previous, candidate);
        }
        internal static void Observe(LicenseList verifiedList, string account) {
            // Caller must verify the issuer signature BEFORE calling this method.
            var candidate = IssuedTicks(verifiedList.issuedUtc);
            string id;
            using (var hash = SHA256.Create()) id = BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(account.ToUpperInvariant()))).Replace("-", "");
            Products.Require(verifiedList.product);
            var folder = StoreDirectory(verifiedList.product);
            var path = Path.Combine(folder, id + ".dat");
            lock (Gate) {
                Directory.CreateDirectory(folder);
                // Exclusive file lock also serializes separate running instances.
                using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) {
                    long previous = 0;
                    if (stream.Length > 0) {
                        if (stream.Length > 4096) throw new UnauthorizedAccessException("本机授权记录无效。");
                        var bytes = new byte[(int)stream.Length];
                        int offset = 0, read;
                        while (offset < bytes.Length && (read = stream.Read(bytes, offset, bytes.Length-offset)) > 0) offset += read;
                        if (offset != bytes.Length) throw new IOException("本机授权记录读取失败。");
                        previous = long.Parse(Encoding.UTF8.GetString(Unprotect(bytes)), CultureInfo.InvariantCulture);
                    }
                    var current = RequireCurrent(previous, candidate);
                    var protectedBytes = Protect(Encoding.UTF8.GetBytes(current.ToString(CultureInfo.InvariantCulture)));
                    stream.Position = 0; stream.Write(protectedBytes, 0, protectedBytes.Length); stream.SetLength(protectedBytes.Length); stream.Flush(true);
                }
            }
        }
    }
}
