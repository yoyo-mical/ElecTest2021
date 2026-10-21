# 共用授权协议与客户端检查点 v1.0.1

仓库：私有 `yoyo-mical/Sim-Timing-Tools-HTML`；分支 `feature/shared-authorization`。
模块：`shared-authorization/SharedAuthorization.csproj`，源码为同目录 Licensing.cs、AuthorizationHistory.cs、IssuerPublicKey.cs。提交 SHA 以本文件首次引入提交为模块 1.0.0 检查点（执行 `git log --diff-filter=A --format=%H -- shared-authorization/PROTOCOL.md`）；后续固定 SHA 记录在 CHECKPOINT.md。1.0.1 仅收紧已 Dispose 的 gate 检查，并使读取 Edition 使用同一会话快照，无线格式或调用 API 变化。

最低目标 **.NET Framework 4.5.2 / C# 5**；无需升级 ElecTest2026 WinForms 主工程。使用框架自带 DataContractJsonSerializer、WindowsIdentity、CurrentUser DPAPI 和 RSACryptoServiceProvider SHA256，不使用仅较新框架提供的 RSA.SignData(HashAlgorithmName, RSASignaturePadding) 重载。可项目引用，也可将三个源码文件加入旧工程并引用 System.Runtime.Serialization、System.Security。客户端只能包含校验模块和既有公钥，不包含管理端源码/私钥。

## 格式与签名

外层 JSON：`{"payload":"Base64(原始JSON字节)","signature":"Base64(RSA签名)"}`。
内层（虚构示例，不是可用授权）：

```json
{"product":"ElecTest2026","issuedUtc":"2026-10-21T06:00:00.0000000Z","users":[{"account":"EXAMPLE-PC\\demo","edition":"Standard","enabled":true,"expiresUtc":"2026-12-01T00:00:00.0000000Z"}]}
```

`product` 必须精确等于 `SimTimingTools` 或 `ElecTest2026`，客户端把预期产品写死，不能从名单反推预期产品。一个文件只能有一个产品，联合操作产生两份独立文件。不接受 ElecTest2021 作为产品标识。

沿用 **RSA PKCS#1 v1.5 + SHA-256** 和原公钥；不轮换密钥。签发端对内层 JSON 的 UTF-8 无 BOM 字节直接签名，然后将同一字节数组 Base64 放入 payload。验证端必须先 Base64 解码并对原字节验签，之后才解析 JSON；不能重排字段、重新序列化、规范化换行、Trim 或改编码后验签。外层可有 UTF-8 BOM，外层缩进不参与签名。生产私钥仅由管理端读取原密钥，不创建替代生产密钥。

## 账号、版本与时间

账号来自 `WindowsIdentity.GetCurrent().Name`，完整 `域或设备名\用户名`，OrdinalIgnoreCase 匹配；不按用户名后缀、不按 SID、不允许通配。当前账号必须唯一命中，重复记录拒绝。名单中两个产品的账号、版本、到期和 enabled 没有继承关系。

`Beta` / `Basic` → `Beta`；`Standard` / `Pro` → `Standard`。其他值拒绝。映射区分大小写，与 Timing 旧规则相同。Beta/Standard 当前功能相同，客户端可依据 Edition 执行已定义功能权限。

空/null/空白 expiresUtc 表示长期；其他值按 InvariantCulture、AssumeUniversal 解析后转 UTC；建议统一写 ISO8601 UTC `Z`。`UtcNow >= expiresUtc` 即到期（到期瞬间不再有效），enabled=false 或账号不存在立即拒绝。日期选择器表示管理机所在时区所选日期结束后到期，签发时必须转换成 UTC，并明确展示该语义。

新签发名单必须包含 issuedUtc，严格递增，停用名单同样递增。旧 Timing 缺少 issuedUtc 可首次读取，按 0 处理；本机见过有时间的新名单后，旧无时间名单也拒绝。ElecTest2026 必须提供 issuedUtc。issuedUtc 是签发版本水位，不是到期时间。

## 启动、刷新、会话

启动默认拒绝，成功 `await gate.RefreshAsync(path)` 后才开放主功能。主进程每 24 小时再次读取（建议更短周期）；核心计算、导出、保存及后台任务入口调用 `gate.Ensure()`，UI 是否启用不能作为唯一检查。Ensure 对当前身份、内存签名、enabled、到期和最长 24 小时会话期限再次检查，不访问共享盘。

刷新成功时原会话撤销并替换；刷新遇到缺文件、读取错误、10 秒超时、签名失败、跨产品、回滚、账号移除/停用/到期或本机记录异常，撤销旧会话并锁定功能，无离线宽限。超时读取的迟到结果不会安装会话。启动和刷新失败均由客户端展示错误并禁止使用；原生计算/导出仍必须检查 Ensure。窗口关闭 Dispose；恢复权限必须重新成功刷新，不恢复旧会话。

未读到远端撤销时无法立刻获知停用，最长延迟取决于刷新周期（默认上限 24 小时）；已发布旧客户端不会自动取得新增规则。不要宣称自由本地文件可实时远程撤销。

## 防回滚隔离

验签及产品匹配之后、账号启用判断之前记录 issuedUtc，因此停用名单和移除账号名单也提高水位。路径：`%LOCALAPPDATA%\<product>\AuthorizationHistory\SHA256(UTF8(account.ToUpperInvariant())).dat`，CurrentUser DPAPI，进程内锁及 FileShare.None 排他文件锁，读取损坏失败关闭，不自动清零。Timing 使用原路径/原 DPAPI 字节格式；ElecTest2026 独立目录，两产品互不推进水位。低于已见 issuedUtc 拒绝，相同水位可重复读取。

这不能防止当前用户删除本机历史、恢复整机快照或改时钟，也不能发现未见过的远端撤销。管理端应使用每份名单原有/已签发时间及当前 UTC 的最大值加最小增量；联合补签只处理失败名单，不重新签成功名单。

## settings 与最小调用

settings 示例见 examples/settings.ElecTest2026.json、settings.SimTimingTools.json。兼容现有 licensePath、administrator、contact 字段；产品在代码固定，不信任可编辑 settings 的 product。相对路径按客户端包根目录解析，完整本地路径及 UNC 均可。

```csharp
// WinForms Form 内；将失败提示和锁定 UI 接入现有主窗体。
private readonly SharedAuthorization.AuthorizationGate auth =
    new SharedAuthorization.AuthorizationGate(SharedAuthorization.Products.Electrical);

// 启动和定时刷新事件（每 24 小时或更短）中：
await auth.RefreshAsync(System.IO.Path.GetFullPath(licensePath));
auth.Ensure();
// 开放界面。任何异常：锁定界面，提示错误；gate 已撤销旧会话。

// 核心计算、导出/保存以及保存对话框确认后：
auth.Ensure();
// 执行操作。长任务及异步任务在提交结果前再次 Ensure。

// FormClosed:
auth.Dispose();
```

## 相对 Timing 的协议变更

无外层封装、签名算法、签名字节、公钥、Timing 旧名单字段或版本映射变更。新增允许的独立产品 ElecTest2026（仅新公共模块按固定预期产品验证，旧 Timing 不接受它）。ElecTest2026 强制 issuedUtc；Timing 保留旧兼容。公共模块不是 ElecTest2026 主工程，此处不会新增或修改该客户端。管理端联合操作分文件提交，无跨文件原子性，必须显示各文件成功/失败并允许失败补签。
