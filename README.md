# ElecTest2026

基于原 ElecTest2021 升级的 C# WinForms + DockPanelSuite 电性测试工具。标题标识 Design by Wuhao Studio。

- `ElecTest2026.sln`：原生工程入口，目标 .NET Framework 4.5.2 / x86。
- `Testplan/Modern/`：原控件布局、矢量操作图标、明亮主题及图表展示调整。
- `Testplan/Authorization/`：固定 ElecTest2026 产品的公共授权模块接入，只含公钥和校验逻辑。
- `design-preview/index.html`：独立HTML设计预览，演示不接触硬件；本地文件不是公开网站。
- `docs/NATIVE-INTEGRATION.md`：协议来源、授权使用、构建方法、验证结果及Windows待验收项。

当前交付是源码检查点。云端缺厂商SDK，尚未生成可正式使用的Windows发布包；临时测试替身不能进入发布。授权管理工具由Sim-Timing环境维护，可单独或联合管理两个产品，各自使用独立签名名单。
