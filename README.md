## 串口交互 HTML 提案（2026-11-13）

[下载单文件 HTML 预览包](https://github.com/yoyo-mical/ElecTest2021/raw/refs/heads/downloads/local-build-20261021/ElecTest2026-serial-ui-preview-20261113.zip)

约437 KiB；解压后双击HTML。仅演示：接收/发送独立模式、原字节HEX/ASCII回看、复制和仅清接收、手动HEX/ASCII文本与结尾选项。不会连接真实设备。本提案尚未更新桌面r5。原Sites网址因当前客户端发布凭据功能不可用，尚未部署此次修改。

## 最新桌面测试包（2026-11-13，r5）

[下载 ElecTest2026 桌面版 r5](https://github.com/yoyo-mical/ElecTest2021/raw/refs/heads/downloads/local-build-20261021/ElecTest2026-desktop-20261113-r5.zip)

约 3.05 MB；完整解压到新目录运行根目录 ElecTest2026.exe。需有效授权及原运行环境。

[本轮更新与验证范围](docs/DESKTOP-R5-20261113.md) · [桌面更新清单](docs/DESKTOP-UPDATE-TRACKER.md)

已同步内部 split、ASCII、RGB/Gray 分组、Sheet 和经典深蓝；运行调度已调整，Windows/WebView2 停止响应及硬件仍需本机验证。以下原有源码检查点说明不代表此 ZIP 的版本。

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
