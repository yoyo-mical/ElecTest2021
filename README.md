## 最新桌面测试包（2026-11-13，r6）

[下载 ElecTest2026 桌面版 r6](https://github.com/yoyo-mical/ElecTest2021/raw/refs/heads/downloads/local-build-20261021/ElecTest2026-desktop-20261113-r6.zip)

3,068,921 字节，约3.07 MB；完整解压到新目录运行根目录 ElecTest2026.exe。需有效授权及原运行环境。

SHA256：`37b30c30f1a27e69be2b8857a1dcc39f559d07bcfc94fef44649e4baef77aa76`

[本轮更新与验证范围](docs/DESKTOP-R6-20261113.md) · [桌面更新清单](docs/DESKTOP-UPDATE-TRACKER.md)

同步 HTML 评审版7的串口独立收发、三处split、六面板展开、测量结果多视图及导出、脚本指令表、面板内操作条、安全边界暂停/继续和原生退出确认。使用真实C#桥接，不含网页模拟设备/测量生成器。40项混淆后后端、虚拟串口字节、浏览器交互与包结构检查通过；Windows/WebView2与真实硬件仍需本机验收。

上一版 [r5](https://github.com/yoyo-mical/ElecTest2021/raw/refs/heads/downloads/local-build-20261021/ElecTest2026-desktop-20261113-r5.zip) 保留用于回退。以下原有源码检查点说明不代表此 ZIP 的版本。

# ElecTest2026

基于 ElecTest2021 升级的电性测试工具，署名 Design by Wuhao Studio。当前入口采用 **C# WinForms 宿主 + WebView2 HTML 界面**，复用已确认的设计与原 C# 业务操作。

- `ElecTest2026.sln`：.NET Framework 4.8 / x86。
- `Testplan/Web/`：正式本地界面，无演示数据或模拟设备连接。
- `Testplan/WebHost/`：浏览器宿主、固定操作桥接、原控件适配。
- `Testplan/Authorization/`：固定 ElecTest2026 产品的公共校验模块，仅含公钥。
- `design-preview/`：独立设计演示；`web-preview/`：浏览器桥接测试截图。
- [当前接入与构建说明](docs/WEBVIEW-INTEGRATION.md)。
- [授权防篡改、混淆与签名发布](docs/RELEASE-HARDENING.md)。

当前交付为源码检查点。云端缺少厂商 SDK 和 Windows 运行环境，尚未生成经过硬件验收的发布包。授权管理工具继续由 Sim-Timing 环境维护。
