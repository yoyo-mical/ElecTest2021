# 原始布局对照图

`original-mono.png` 是 1600 × 980、Mono/Xvfb 下根据原始 WinForms Designer 控件和资源生成的界面截图。它不是原程序在 Windows 上完整运行的截图，也不是 HTML 重画图。

直接使用未修改的 UART、CA410、ARMCode、Control、Pattern、MainForm Designer，以及原始 Properties.Resources。保留 MainForm 原始 DockTo、DockAlignment、0.19/0.25 左右比例、中央 0.48/0.53 分割关系。

为避开当前环境的已确认限制，独立截图程序位于 `/workspace/.elec-env/baseline`：

- 各子窗体只执行 InitializeComponent；事件处理为空，不执行仪器、串口、配方业务逻辑。因而组合框未经过原 Load 方法填充，没有实际测量结果。
- 对 MainForm 的外部兼容副本使用 DockingWindow 文档模式并关闭 Mono 不兼容的 MDI；保持原始区域分割。启用 DockPanelSuite 补丁开关，避免只读注册表查询；可见 DockWindow 调整 Z 顺序，解决 Mono 的侧栏遮挡。
- Xvfb 没有窗口管理器，因此显式设置全屏边界，代替窗口管理器的最大化动作。
- Mono Chart.BeginInit 未实现，图表内部使用同位置的浅黄色占位面板，图中明确写明；外部 DataChart 页签和布局来自原 Designer。
- 字体、系统控件皮肤和 DPI 行为与 Windows 可能不同。本图用于核对区域、控件和操作入口，不用于宣称 Windows 像素一致性或业务功能通过验证。

原仓库已有 C# 文件、Designer、项目与资源没有修改。当前 screenshot harness 编译成功并完成截屏；日志仍含 Mono/X11 兼容性诊断，不能视作主程序运行验证。

对照第一版 HTML 预览已发现的简化/差异：

1. 原 Pattern 定义 pictureBox1–pictureBox32 共 32 个图案控件；第一版预览只有 18 个近似绘制图案，不能当作完整原图案库。
2. 原 Control 有 Patten 下拉及参数输入、DownLoad 内容输入、选择文件与路径输入；第一版预览未逐项保留。
3. 原 Pattern 的 Back 是数值输入，第一版预览做成了复选框，语义需要纠正。
4. 原 Control 配方区是可编辑富文本；预览改为步骤表格，是设计提案，需要保留/确认原自由文本能力。
5. 原控制栏区分从头运行、从当前行运行、单步，另有格式刷；预览尚未逐项呈现这些独立入口。

以上是已知待完善项，不应认为“六个面板都在”就代表所有功能已完整映射。当前请求先交付原布局图片用于比对，没有在此步骤擅自重做第一版设计。

## 后续预览补齐状态

上面的五项差异为首版记录。本轮预览已补齐 32 个原控件/资源/Tag、Control 参数/下载/文件路径入口、数值 Back、独立自由文本配方及独立运行/格式刷入口。详见 README.md 本轮记录及 PATTERN-MAPPING.md。预览不执行命令，不等同于原生富文本文件或硬件功能验证；original-mono.png 保留为原兼容截图。
