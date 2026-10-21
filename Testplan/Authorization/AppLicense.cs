using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedAuthorization;

namespace Testplan {
    [DataContract]
    internal sealed class LicenseSettings {
        [DataMember] public string licensePath = "";
        [DataMember] public string administrator = "Wuhao Studio";
        [DataMember] public string contact = "请联系管理员开通 ElecTest2026 权限";
    }
    // One fixed product and production public key; no configurable trust/test bypass.
    internal static class AppLicense {
        private static readonly AuthorizationGate Gate = new AuthorizationGate(Products.Electrical);
        private static readonly object Sync = new object();
        private static bool admitted, disposed;
        private static string account = "", edition = "";
        internal static event Action<string> Locked;
        internal static string Account { get { lock (Sync) return admitted ? account : Licensing.CurrentAccount(); } }
        internal static string Edition { get { Ensure(); lock (Sync) return edition; } }
        internal static string ConfigPath { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"); } }
        internal static LicenseSettings ReadSettings() {
            if (!File.Exists(ConfigPath)) return new LicenseSettings();
            return JsonCodec.Deserialize<LicenseSettings>(File.ReadAllText(ConfigPath));
        }
        internal static async Task RefreshAsync(string path) {
            lock (Sync) {
                if (disposed) throw new ObjectDisposedException("AppLicense");
                if (admitted) return; // A successful startup grant lasts for this process only.
            }
            try {
                if (string.IsNullOrWhiteSpace(path)) throw new IOException("请设置 ElecTest2026 授权名单路径。");
                var full = Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
                WebIntegrity.Ensure();
                await Gate.RefreshAsync(full);
                var verifiedEdition = Gate.Edition;
                var verifiedAccount = Licensing.CurrentAccount();
                lock (Sync) {
                    if (disposed) throw new ObjectDisposedException("AppLicense");
                    account = verifiedAccount; edition = verifiedEdition; admitted = true;
                }
                Gate.Revoke(); // Do not retain a renewable/timed session during measurement.
            } catch (Exception ex) { Lock(ex.Message); throw; }
        }
        internal static void Ensure() {
            // Startup admission guard only: no RSA, clock, file, network or identity reads.
            lock (Sync) {
                if (disposed) throw new ObjectDisposedException("AppLicense");
                if (!admitted) throw new UnauthorizedAccessException("请先完成启动授权检查。");
            }
        }
        internal static bool AllowOperation() {
            try { Ensure(); return true; }
            catch { return false; }
        }
        internal static void Lock(string reason) {
            lock (Sync) { admitted = false; edition = ""; }
            Gate.Revoke(); var handler = Locked; if (handler != null) handler(reason);
        }
        internal static void SavePath(string path) {
            Ensure();
            var settings = ReadSettings(); settings.licensePath = path;
            File.WriteAllText(ConfigPath, JsonCodec.Serialize(settings));
        }
        internal static void Dispose() { lock (Sync) { disposed = true; admitted = false; edition = ""; } Gate.Dispose(); }
    }
}
