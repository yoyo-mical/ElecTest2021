# ElecTest2026 桌面下载

最新测试版：[r9 下载](https://raw.githubusercontent.com/yoyo-mical/ElecTest2021/desktop-downloads/ElecTest2026-desktop-20261116-r9.zip)

包大小3,220,187字节；SHA256 `ec045537368eaca98383e52a2f4500cd0b67f7b710721e7a94fb5f7b63090b0d`。旧r8包保留。

# ElecTest2026 r9 · DPI处理测试版

基于r8的HTML35桌面功能，新增窗口创建前主动请求Per-Monitor V2，首次显示/跨屏DPI变化诊断与WebView2原生布局刷新。启动日志记录实际生效的DPI模式，便于定位整窗模糊；这不代表Windows4K实机问题已经验证解决。

同时包含独立的“界面”缩放100/115/125/150/175/200%，默认100%，根settings.json的uiScale可保存；它只用于调整整体大小，不能代替DPI清晰度修复。字体大小设置仍独立存在。

授权名单、联系人和操作方式沿用r8。完整解压后，将旧包根settings.json中的licensePath复制到新版配置即可；勿覆盖程序及运行组件。使用原有外置WebView2Runtime时仍放在新版“程序及运行组件/WebView2Runtime”目录。

本版重点请在27英寸3840×2160、Windows175%或200%下检查文字与控件边缘，以及跨屏后清晰度。界面缩放先保持100%，与r8对比；若仍模糊，请提供%LOCALAPPDATA%\ElecTest2026\startup.log。日志不包含授权名单正文。

真实厂商SDK编译、配置兼容、Web完整性和桌面UI同步检查通过；混淆与后端/虚拟串口验证记录见交付文档。Windows真实启动/4K/2K、设备及文件窗口仍待本机确认。未提供发布者签名，不捆绑WebView2 Runtime。测量记录未导出或Save前仅在内存，退出不自动恢复。
