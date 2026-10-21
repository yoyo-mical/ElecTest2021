> 历史记录：本文描述已被替换的原生布局方案。当前入口、框架版本、授权页及构建方式以 [WebView2 接入说明](WEBVIEW-INTEGRATION.md) 为准。

# ElecTest2026 原生界面与授权接入

当前为源码实现检查点，不是已完成 Windows/仪器验收的安装包。HTML 设计预览仍在 design-preview；原生界面在 Testplan/Modern，以重新布局现有控件的方式接入，不复制一套测量流程。

## 公共授权来源

公共校验模块 1.0.1：yoyo-mical/Sim-Timing-Tools-HTML 的 feature/shared-authorization，固定提交 c531b86dacdcbb16713d113bded80f75e95fa5cf。根据该环境 CHECKPOINT.md 允许公开的交付范围，仅引入 Licensing.cs、AuthorizationHistory.cs、IssuerPublicKey.cs 和 PROTOCOL.md；没有管理端、私钥、真实名单、管理员密码或签发能力。

模块源码位于 Testplan/Authorization/Shared，协议见该目录 PROTOCOL.md。三个 C# 源码直接加入旧式工程，引用 System.Runtime.Serialization 与 System.Security；目标仍是 .NET Framework 4.5.2。AppLicense 固定 Products.Electrical（ElecTest2026），settings 不能改变产品或公钥。原生窗口名称和输出程序集为 ElecTest2026，内部 Testplan 命名空间保留以兼容资源名和原事件。

## 授权使用

1. 管理员从同一管理工具签发 ElecTest2026 独立名单；Timing 名单不被本程序接受。
2. 启动后在底部填写名单绝对路径、UNC 路径或相对 EXE 目录的路径，点击“检查授权”。默认读取 EXE 旁 settings.json 的 licensePath。示例配置不包含可用名单。
3. 仅成功验签、产品/Windows账号匹配、期限和本地防回滚校验后，才创建六个功能面板；没有授权不初始化硬件工作区。
4. 勾选“验证后保存路径”才写 settings.json；目录只读时可取消勾选，本次仍可检查。路径保存失败会锁定，提示真实错误。
5. 每小时刷新共享名单，每分钟检查当前会话。关键发送、测量、配方运行、结果提交与导出入口也检查；导出对话框确认后再次检查。刷新读取失败/超时、账号停用/到期等撤销旧会话并锁定工作区。恢复需重新成功检查。
6. “断开设备”在锁定状态仍可用，调用原断开流程；授权丢失停止后续配方步骤，不插入未知复位命令，不更改延时/采样设置。正在同步执行的 SDK 调用允许返回后再阻止结果提交及后续操作。关闭时停止计时器、撤销授权、关闭串口；CA410 可先通过断开设备按钮清理。

未读到的新停用名单无法实时获知；名单刷新周期为一小时。自由本地副本及用户删除 DPAPI 历史等限制沿用公共协议，不宣称彻底防破解或即时远程撤销。

## 原生布局

- 六个 DockContent：UART/CA410 左侧上下，ARMCode/DataChart 中间上方，Control 中间下方，Pattern 右侧。保留 DockPanelSuite 停靠、分割及浮动；关闭按钮禁用以防误丢业务窗体。
- 三种明亮主题，标题显示 ElecTest2026 与 Design by Wuhao Studio。使用标准系统窗口边框，减少自绘窗口缩放和跨屏问题。manifest 声明 per-monitor DPI，MainForm 接收 WM_DPICHANGED 缩放并限制窗口最小尺寸；仍需 Windows 验证，不能把 Mono 布局检查当成跨屏 DPI 验收。
- ARMCode 原富文本、Code/Sheet、格式化/注释/下载/删除、原数据表及导出入口保留；六项数值在同一行。
- Control 保留原 RichTextBox，不强制转换为步骤表；所有原 ITEM、参数、Patten 预设、Pattern/DownLoad 内容、DownCOM、文件/路径、GoTo、编写、格式刷、上移/下移/删除、独立从头/当前行/单步入口保留。高级输入折叠展示；时序参数继续遵守原代码单位，未按预览示例擅自改单位。
- Pattern 保留32个 PictureBox 的原资源和 Tag、原参数启用与发送处理。数值 Back、RGB/Gray 原标签操作保留。列表上方为分辨率/RGB/筛选，下面是默认收起的大预览；预览无发送处理。移除纯视觉的悬停放大以保证网格稳定，键盘/触屏点击会先解析所选原 Tag。
- 串口状态根据实际 Open/Close 更新；CA410 状态综合原 SDK 返回码及异常，不以点过连接按钮当作成功。模式、读数、SDK 设置/测量顺序保持原样。
- Chart1 继续消费原配方事件与样本。默认折线+实测点，最新点高亮、点击读数、横轴选择缩放和恢复、可选明确标注的平滑插值。Chart2 未实现历史对比，因为该用途尚未确认。未填入演示测量数据。
- 操作图标用 Drawing2D 矢量绘制，图案资源仍保持原图。

## 构建

Windows 上安装 Visual Studio/MSBuild、.NET Framework 4.5.2 开发包和原厂 CASDK2（包括要求的 Windows 驱动/COM 组件）。Debug/Release 固定 x86，与原 CASDK2Net 引用架构一致。

原厂程序集可放原来的 Testplan/bin/Debug，或明确指定目录：

```powershell
MSBuild.exe ElecTest2026.sln /t:Rebuild /p:Configuration=Release /p:VendorSdkDirectory="D:\VendorSDK"
```

目录应提供原 CASDK2Net.dll（以及旧项目引用的 Admesy.dll）。不要用 tests 的替身程序集构建发布版。输出 Testplan/bin/Release/ElecTest2026.exe、settings.json、配置和依赖；生产名单由管理员另外提供，不进入公开源码。没有升级.NET框架、没有将管理工具打进客户端。

## 本环境验证与限制

运行 `bash tests/run-native-checks.sh`（需要云环境原有 Mono/Xvfb；临时 SDK 替身只在/tmp，脚本不会覆盖正式 SDK 或发布产物）：

- 17项临时密钥授权测试，包含跨产品/账号、到期/停用、签名篡改、历史回退隔离、旧版本映射、启动默认拒绝、刷新失败撤销、关闭拒绝。Linux 身份与 DPAPI 在临时源码中替换，生产三份源码哈希保持不变，临时私钥自动删除。
- 逐文件对照旧基线84210471974c6c16cb517ddec8269540b75114e2，发送协议字符串、延时表达式和原SDK调用序列一致；新授权检查可能增加操作入口开销，不能据此声称实测采样周期完全一致。
- 托管源码编译通过，存在原有未使用成员警告。仅在临时编译副本把缺失SDK的对象类型改成dynamic及禁止调用的SDK替身；不能证明真实CASDK方法签名、COM加载或实际仪器工作。
- 原生六面板布局检查、ARM单行参数、编辑区高度、32原图案、默认折叠及富文本保留检查通过。无授权启动保持Dock禁用且未创建任何功能面板。

正式工程在本环境构建仍因缺 CASDK2Net.dll/厂商依赖失败。没有发布可用EXE或安装包。native-preview/layout-mono.png 为独立原生布局验证窗口，图表明确标为Mono占位；startup-locked-mono.png 为启动锁定界面。它们不是Windows完整运行截图。

Windows验收必须覆盖：实际名单/账号/DPAPI、UNC断开及恢复、停用/到期时正在运行的配方、锁定后的断开设备、所有原配方入口与数据导出、串口收发、CA410真实返回码/测量模式、Chart1真实参数扫描/VCOM/Gamma、32图案、不同分辨率和100/150/200%缩放、拖动跨屏后的字体/窗口/停靠比例。
