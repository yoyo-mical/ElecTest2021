# 分发程序加固

本次保持公共授权模块1.0.1、ElecTest2026产品标识、公钥和名单格式不变，无需授权工具重新签发。已有关键业务入口继续验签和检查会话；页面的 authorized 字段只是显示状态，不能授予 C# 操作权限。

## 已实现

- 编译进主程序集的 SHA-256 清单覆盖 Web 目录所有文件。启动 WebView2 前检查，后续授权操作每30秒最多复查一次；缺失、修改或额外文件导致失败并撤销会话。首次发现完整性失败后当前进程持续拒绝，需修复文件并重启。校验使用单调计时，不受系统时间回拨影响。
- `security/obfuscar.xml` 使用 Obfuscar 2.2.50 隐藏私有标识符及字符串。保留序列化模型、资源访问和公共API以兼容原 WinForms/SDK。并非所有名字都被混淆，也不隐藏 HTML 源码。
- Release 不生成 PDB。受保护构建脚本将混淆映射留在私有目录，发布目录只复制程序、依赖、配置及 Web，避免复制源文件、调试符号、现有授权路径与名单。
- `tools/build-protected.ps1` 在真实 Release 构建、混淆后执行 Authenticode 签名并验证；任一步失败停止。代码签名证书与授权签名密钥分离，脚本只接受 Windows 证书库指纹，不复制私钥。验证发布者签名可发现签名后被修改的EXE，但Windows默认并不阻止用户运行未签名替代程序。

## 使用

修改 Web 后，审查变更，再运行 `python tools/generate-web-integrity.py` 更新编译清单。不要在客户程序启动时自动重建清单。普通构建若未更新清单，运行时将拒绝改变后的页面；受保护构建在编译前直接检查清单一致性。

在具备 .NET Framework 4.8 开发包、原厂SDK、NuGet、Python和Windows SDK signtool的 VS Developer PowerShell 中：

```powershell
.\tools\build-protected.ps1 -VendorSdkDirectory D:\VendorSDK -CodeSigningThumbprint <证书库中的代码签名证书指纹>
```

脚本使用当前用户证书库，生成独立 artifacts/build-*/package 签名候选目录；private-obfuscation 中的映射留作内部诊断，绝不能随包交付。发布前必须对这份混淆后的候选包做 Windows/WebView2/真实仪器回归。不要把普通 bin/Release 输出当成已经混淆和签名的交付物。当前环境没有代码签名证书与厂商SDK，因此未生成签名生产包。

## 验证与边界

`tests/check-web-integrity.py` 验证原文件通过、修改/删除/新增文件被拒绝、还原后通过。`tests/run-web-checks.sh` 覆盖原有授权、协议时序、后端和浏览器检查。设置 ELEC_OBFUSCAR 为 Obfuscar.Console.exe 路径后运行 check-web-backend.py，会先混淆临时测试程序集，再运行同样的17项后端检查，包括失效授权拒绝执行/发布结果、JSON序列化、原配方、资源初始化与事件释放。测试替身只在临时目录，不用于生产。

这些措施提高修改成本，不构成不可绕过的安全边界。拥有本机控制权的人仍可能修改整个EXE、内存或校验代码；内置清单本身也可随EXE被替换。30秒复查存在检测窗口，不能阻止文件校验后即时替换。离线名单的撤销时效、账号与DPAPI限制仍沿用共享协议。更强的信任边界需要服务端参与或受管系统强制代码签名策略，涉及另一个授权工具环境，应单独设计协议，不在本次擅自改变离线使用方式。

混淆保护的是分发二进制。如果源码仓库公开，代码依然可直接阅读；本次未修改仓库可见性。未加入检测调试器后强杀进程等行为，以免打断测量或影响维护。
