# ElecTest2026 桌面下载

最新测试版：[r10 下载](https://raw.githubusercontent.com/yoyo-mical/ElecTest2021/desktop-downloads/ElecTest2026-desktop-20261116-r10.zip)

包大小3,220,242字节；SHA256 `9ec5525d17e210aced8b2c4294d2471dfe5ba1ec38fbc815a3e12f63cd1bcfa0`。旧r8/r9包保留。

# ElecTest2026 r10 · 配方运行同步最新脚本

修复r9及此前版本的输入同步遗漏：配方“从头运行、从当前行运行、单步”原来只提交recipe字段，没有提交编辑区最新arm.script及参数。后端因此仍使用旧脚本/空模块索引；先点Run Code碰巧同步了脚本，掩盖问题。

三种执行入口现在一次性提交最新完整脚本、参数及配方，再由既有C#模块索引处理执行。不需要先点Run Code或格式化，不额外下发整份脚本；真正下发仍按配方步骤执行。导入、排版等非执行配方操作仍保持原范围。

保留r9的DPI处理、诊断及可选整体界面缩放。使用时完整解压新版并配置根settings.json的licensePath；外置WebView2Runtime仍放在组件目录。程序未签名、不包含运行时或真实授权名单。Windows4K清晰度及真实仪器执行仍需本机验证；未导出/Save的结果仍只在内存。

验证：浏览器桥接回归验证三个入口携带最新脚本/参数且没有隐式arm.download；原生回归验证新模块索引不依赖Run Code。虚拟串口回归不代表真实仪器测试。真实厂商SDK构建并沿用私有符号混淆和完整性清单。
