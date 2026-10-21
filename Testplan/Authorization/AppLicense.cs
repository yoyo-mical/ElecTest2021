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
        internal static event Action<string> Locked;
        internal static string Account { get { return Licensing.CurrentAccount(); } }
        internal static string Edition { get { return Gate.Edition; } }
        internal static string ConfigPath { get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"); } }
        internal static LicenseSettings ReadSettings() {
            if (!File.Exists(ConfigPath)) return new LicenseSettings();
            return JsonCodec.Deserialize<LicenseSettings>(File.ReadAllText(ConfigPath));
        }
        internal static async Task RefreshAsync(string path) {
            try {
                if (string.IsNullOrWhiteSpace(path)) throw new IOException("请设置 ElecTest2026 授权名单路径。");
                var full = Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
                WebIntegrity.Ensure();
                await Gate.RefreshAsync(full);
                Gate.Ensure();
            } catch (Exception ex) { Lock(ex.Message); throw; }
        }
        internal static void Ensure() {
            try { WebIntegrity.Ensure(); Gate.Ensure(); }
            catch (Exception ex) { Lock(ex.Message); throw; }
        }
        internal static bool AllowOperation() {
            try { Ensure(); return true; }
            catch { return false; }
        }
        internal static void Lock(string reason) {
            Gate.Revoke(); var handler = Locked; if (handler != null) handler(reason);
        }
        internal static void SavePath(string path) {
            Ensure();
            var settings = ReadSettings(); settings.licensePath = path;
            File.WriteAllText(ConfigPath, JsonCodec.Serialize(settings));
        }
        internal static void Dispose() { Gate.Dispose(); }
    }
}
