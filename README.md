# ElecTest2026

基于 ElecTest2021 升级的电性测试工具，署名 Design by Wuhao Studio。当前入口采用 **C# WinForms 宿主 + WebView2 HTML 界面**，复用已确认的设计与原 C# 业务操作。

- `ElecTest2026.sln`：.NET Framework 4.8 / x86。
- `Testplan/Web/`：正式本地界面，无演示数据或模拟设备连接。
- `Testplan/WebHost/`：浏览器宿主、固定操作桥接、原控件适配。
- `Testplan/Authorization/`：固定 ElecTest2026 产品的公共校验模块，仅含公钥。
- `design-preview/`：独立设计演示；`web-preview/`：浏览器桥接测试截图。
- [当前接入与构建说明](docs/WEBVIEW-INTEGRATION.md)。

当前交付为源码检查点。云端缺少厂商 SDK 和 Windows 运行环境，尚未生成经过硬件验收的发布包。授权管理工具继续由 Sim-Timing 环境维护。
