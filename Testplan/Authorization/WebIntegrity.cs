using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Testplan {
    // Integrity raises the cost of replacing local UI scripts. It is not an external root of trust.
    internal static class WebIntegrity {
        private static readonly object Sync = new object();
        private static readonly Stopwatch Clock = Stopwatch.StartNew();
        private static long nextCheck;
        private static bool failed;
        internal static void Ensure() {
            lock (Sync) {
                if (failed) throw new InvalidOperationException("程序界面完整性校验失败，请重新安装可信发布包。");
                if (nextCheck > Clock.ElapsedMilliseconds) return;
                try {
                    VerifyDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web"));
                    nextCheck = Clock.ElapsedMilliseconds + 30000;
                } catch { failed = true; throw; }
            }
        }
        internal static void VerifyDirectory(string root) {
            var expected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in WebIntegrityData.Entries) {
                var parts = entry.Split('|');
                var relative = parts[1].Replace('/', Path.DirectorySeparatorChar);
                expected.Add(Path.GetFullPath(Path.Combine(root, relative)));
                using (var stream = File.Open(Path.Combine(root, relative), FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var hash = SHA256.Create()) {
                    var actual = BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                    if (!string.Equals(actual, parts[0], StringComparison.Ordinal))
                        throw new InvalidOperationException("程序界面文件已改变，请重新安装可信发布包。");
                }
            }
            foreach (var file in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
                if (!expected.Contains(Path.GetFullPath(file)))
                    throw new InvalidOperationException("程序界面目录含未受信任文件，请重新安装可信发布包。");
        }
    }
}
