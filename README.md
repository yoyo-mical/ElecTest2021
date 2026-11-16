# ElecTest2026 桌面下载

本分支只提供桌面测试包，不包含开发源码或授权名单。

下载：[ElecTest2026-desktop-20261116-r8.zip](https://raw.githubusercontent.com/yoyo-mical/ElecTest2021/desktop-downloads/ElecTest2026-desktop-20261116-r8.zip)

SHA256：`95cb459b2009f33e4c2b6d4329471cbd7abc244c86da466b143b2bb20db31e10`

包大小：3,218,928字节；HTML35；当前仍待Windows/硬件验收。完整解压后，配置根settings.json并运行根ElecTest2026.exe。

# ElecTest2026 桌面测试包 r8 · HTML35同步

本包同步已评审HTML35界面到真实C#桌面适配：六面板精简/更多/全屏与内部拖动；UART多行发送校验、收藏及有界近期日志；CA410马蹄图、白点、历史、RGB色域采集；ARMCode模块编辑、最新参数Run Code、HEX步进/校验与脚本Sheet；配方布局/按类型参数/Goto/单按钮暂停继续；任意组单图、Sheet、图加表、多图选择布局调大小排序、同类叠加、按组或点删除及20次撤销、统一真实结果导出。

测试图案44项按最新顺序和分类，保留32个原图案，10项文档依据预设与黑白黑文档推导预设接受控C#映射。3/4白框仅预览，无已确认发送指令；黑白黑的7E资料冲突仍需设备核对。

主题顺序：经典深蓝、时尚暗黑、柔和暖白、中性浅灰；字体100/115/130%可保存。保留PerMonitorV2，无整页transform，WebView2原生缩放；有效授权在启动校验后直接进入，失败才提示配置联系人，正常界面移除授权/技术标识。

新界面以毫秒添加等待步骤使用`DelayMs`；旧`Delay`仍按秒执行，保留旧配方含义。暂停在安全检查点生效，当前不可中断的SDK调用仍需返回；不强杀线程。测量数据未导出或未执行Save前仅在进程内存中，退出不会自动恢复。文件日志轮转、自动保存、原始JEITA波形/频谱、组包/等待、自动重试仍不加入本版。

验证：真实厂商SDK x86编译、51项原生桥接/结果/校验/日志检查、生成桌面UI的桥接交互、实际虚拟串口字节（含11项新增图案）、Web完整性核验。主程序沿用Obfuscar私有符号混淆，关闭曾导致Windows初始化异常的字符串隐藏；序列化契约保持。包内无源码、PDB、混淆映射、实际授权名单或开发机license路径。

**Windows/WebView2真实启动、2K/4K及不同DPI/跨屏清晰度、原厂设备和长配方时序、原生文件保存窗口仍待本机验收。Linux编译、桥接截图和虚拟串口检查不等于Windows/硬件通过。测试包未提供发布者数字签名。**

完整解压后编辑根`settings.json`的licensePath，双击根`ElecTest2026.exe`。根目录保持exe、settings.json和程序及运行组件三项；依赖.NET Framework4.8、WebView2 Runtime、原厂SDK/驱动及x86 VC++运行库。出现启动异常运行组件目录的START-DIAGNOSTIC.cmd，并查看%LOCALAPPDATA%\ElecTest2026日志。
