# WebView2 界面与授权接入

当前入口为 `Program → WebWorkspaceForm`，使用本地 HTML/CSS 显示六面板工作台。沿用确认的设计、三种主题、Design by Wuhao Studio 标识和32个原图案。原 `Modern` 原生布局保留在源码中，但不再作为启动界面；此前 native-preview 截图不代表本版效果。

## 界面和业务边界

`tools/build-web-ui.py` 从 design-preview 提取布局和原图资源，删除演示脚本，生成 Web/index.html、patterns.js 和来源记录。desktop.css 与 native.js 为正式适配代码。修改设计源后运行生成脚本；不要直接在生成的 HTML 中维护业务逻辑。

WebView2 仅加载 electest.local 本地映射，外部导航、窗口、下载、权限和网络资源被拦截。桥接使用 JSON 数据与固定操作白名单，页面不能调用任意 C# 方法。无原生宿主或无有效授权时工作区锁定。

UART、CA410、ARMCode、Control、Pattern 继续调用原 C# 处理方法。串口枚举不自动探测打开设备。脚本及配方按原文处理，富文本用安全文本片段保留粗体、斜体、颜色；步骤表是同一原文的展示。保留从头、当前行、单步、格式刷、文件选择、路径、下载内容及原参数。ARM 六项参数压为一行，Pattern 大预览在列表下方默认收起，Back 为数值。

Chart1 订阅原配方样本事件，仅显示 C# 返回的实际样本；没有样本时保持空图。参数扫描、VCOM 与 Gamma 仍由当前测试决定；平滑线明确标为插值。Chart2 历史对比尚未实现。连接状态来自实际状态；CA410 测量返回错误时不发布本次读数。

原设备调用保留在 STA 线程，未搬到浏览器或后台线程。桥接增加入口开销，实际采样周期仍需硬件验证。同步设备调用不能强行中断；停止操作会阻止后续步骤，运行中不因授权到期中断。当前 HTML 工作台支持分隔调整及面板展开，尚未实现原 DockPanelSuite 的浮动窗口。原文件选择和设备错误等对话框仍为原生窗口。

## 授权

沿用公共模块1.0.1，来源 Sim-Timing-Tools-HTML 的 feature/shared-authorization，提交 c531b86dacdcbb16713d113bded80f75e95fa5cf。产品固定 ElecTest2026，密钥、公钥和签名协议没有因界面改造改变。仅引入允许公开的校验代码，未引入管理端、私钥或真实名单。

启动授权页读取 settings.json 的 licensePath，也可填写名单路径并选择保存。通过签名、产品、Windows账号、有效期及防回滚校验后才创建业务对象。只在启动解锁时验证名单，成功后保存仅当前进程有效的准入状态；运行中不刷新名单、不检查有效期、不重复验签。下次启动重新校验。启动未通过和关闭后仍禁止业务操作，停止及断开保持可用。联合授权由同一管理工具签发各产品独立名单，Timing名单不能用于本软件。

## Windows 构建

安装 Visual Studio/MSBuild、.NET Framework 4.8 开发包、NuGet、原厂 CASDK2/COM/驱动与所需 Admesy.dll。保留 x86，与厂商SDK一致。

```powershell
nuget restore ElecTest2026.sln
MSBuild.exe ElecTest2026.sln /t:Rebuild /p:Configuration=Release /p:VendorSdkDirectory="D:\VendorSDK"
```

WebView2 SDK 固定1.0.4258.31，与参考项目一致，因此目标框架升至4.8。运行机安装 WebView2 Runtime；也可在程序旁 WebView2Runtime 放完整 x86 Fixed Version Runtime，宿主校验文件和架构后回退使用。不能只复制浏览器EXE。

输出必须包含 ElecTest2026.exe、配置、依赖程序集、x86 WebView2Loader.dll 和整个 Web 目录。正式构建必须使用真实厂商DLL，禁止携带 tests 临时替身。不要把测试编译结果当作发布包。

## 验证与未完成验收

`bash tests/run-web-checks.sh` 运行协议/时序源码不变量、临时密钥授权测试、原C#后端桥接测试和浏览器交互测试。测试依赖 Mono、Xvfb、Python cryptography/Playwright 与 Chromium。临时身份、密钥和禁止调用的SDK替身仅在 /tmp 生成，生产共享源码保持不变。

浏览器测试覆盖无宿主锁定、六面板、32图案、原操作映射、无预置数据、富文本换行、样本绘图、插值标识、忙碌/停止、锁定状态展示、三主题与1280/1366/1920宽度。web-preview 截图明确标记“桥接测试 · 无硬件”，不是Windows或仪器验收截图，也不是公开网站。

仍需 Windows 验证：真实SDK编译、WebView2启动及部署、COM加载、账号/DPAPI/UNC授权、运行中停用或到期不中断且下次启动拒绝、串口收发、CA410三模式、所有配方入口与导出、32图案实际输出、Chart1真实数据，以及100/150/200% DPI和跨屏缩放。云端目前不能完成这些验收。

分发前请使用 [受保护构建流程](RELEASE-HARDENING.md)，执行界面完整性检查、混淆与发布者签名；普通 Release 编译不会自动完成混淆和签名。

当前产品策略：启动授权成功后，即使名单不可达、管理员停用或授权到期，本次运行仍可继续使用；这些变化在下次启动时生效。原共享模块源码和签名协议未改，客户端不再持有其有时限的运行会话。此策略由用户明确选定。
