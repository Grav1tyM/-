# Deadlock 比赛筛选

一个给 Windows 用的绿色桌面程序。输入 Steam 好友码，到 [Statlocker](https://statlocker.gg/) 找出这个玩家，再列出爆头率和命中率都高于你所设阈值的比赛。

程序在使用者自己的电脑上运行。它只访问 Statlocker 的公开接口，不需要在作者的电脑上开任何服务，也不需要先安装 Python 或 Node。

详细的构建和打包说明在 [desktop/使用说明.txt](desktop/使用说明.txt)。开发说明在下面。

## 使用者怎么运行

1. 在 Windows 上构建出 `DeadlockMatchFilter.exe`（步骤见下一节），或从本仓库的 GitHub Actions 产物里下载。
2. 新建一个文件夹，放进这个 exe，也可以把 [desktop/使用说明.txt](desktop/使用说明.txt) 放在旁边。
3. 把文件夹打成 zip。对方解压后双击 `DeadlockMatchFilter.exe`。
4. Windows 10/11 需要系统里的 [WebView2 运行时](https://developer.microsoft.com/microsoft-edge/webview2/)。较新的系统已经自带。这是 Windows 组件，不是我们的服务器。

## 在 Windows 上构建 exe

需要：

- Node.js 22
- Rust stable（rustup）
- Visual Studio 生成工具，勾选「使用 C++ 的桌面开发」

在仓库里：

```bat
cd desktop
npm install
npm test
npm run tauri build
```

绿色版 exe 在 `desktop\src-tauri\target\release\`。`npm run tauri build` 会把它命名为 `DeadlockMatchFilter.exe`。只跑 `cargo build` 时文件名是 `deadlock-match-filter.exe`，两个都是同一个程序。

Tauri 还会打一个当前用户的 NSIS 安装包，在 `desktop\src-tauri\target\release\bundle\nsis\`。解压即用时用 release 目录里的 exe，不要用安装包。打包 zip 时把这个 exe 和 `desktop\使用说明.txt` 放在同一个文件夹里。

当前这台构建环境是 Linux，不能在这里直接产出 Windows exe。上面的命令在 Windows 上会产出 exe。

## 怎么找到玩家和比赛

好友码按 Statlocker 网页自己的查法处理，没有另造接口。2026-11-04 对线上站点核对过：

1. 数字好友码就是 Steam 账户 ID。程序请求 `GET https://statlocker.gg/api/profile/steam-profile/{好友码}`。这个接口也接受 17 位 Steam64。找不到时返回 404 或空内容，界面提示没有这个玩家。
2. 不要把纯数字交给 `search-profiles`。那个接口是按名字搜的，数字会被当成昵称。
3. 找到玩家后，用返回的 `accountId` 请求 `GET https://statlocker.gg/api/profile/data/matches/{accountId}/true`。这是个人主页「全部比赛」用的数据。每一场有 `match_id`、`accuracy`、`headshotAccuracy`。后两项是 0 到 1 的比例，没有数据时是 null。
4. 比赛页是 `https://statlocker.gg/match/{match_id}`。点击表格里的比赛代码，会用系统浏览器打开这一页。

不带 `Origin: https://statlocker.gg` 时，这些地址会返回 401，并提示缺少 API key。网页自己的请求带有这个来源。程序带上同样的 Origin 和 Referer，不使用私有密钥。

也接受这些等价写法，都会先换成上面的账户 ID 再查询：`STEAM_X:Y:Z`、`[U:1:数字]`、`https://steamcommunity.com/profiles/数字`。自定义网址 `steamcommunity.com/id/...` 不是好友码，程序会直接提示。

没有比赛记录时，接口仍返回 200，`matchHistory` 为空。界面会说明记录为空，而不是当成网络错误。

## 阈值和优先指标

两个阈值的单位都是百分比。一场比赛要同时高于两个阈值才会出现，等于不算高于。

优先指标不放宽条件，只决定排序：

1. 先按优先的那一项从高到低。
2. 相同再按另一项从高到低。
3. 仍相同则较新的比赛在前。

举例：爆头率阈值 20，命中率阈值 50，优先爆头率。

| 爆头率 | 命中率 | 结果 |
| --- | --- | --- |
| 25% | 60% | 列入 |
| 25% | 50% | 不列入，命中率只是等于阈值 |
| 30% 与 22% | 55% 与 70% | 两场都列入，30% 那场排在前面 |

缺少爆头率或命中率的比赛不列入，并单独计数。

## 目录

- `desktop/core`：好友码解析，以及 Statlocker 请求和 JSON 解析。`cargo test --manifest-path desktop/core/Cargo.toml`
- `desktop/src`：界面和筛选排序。`cd desktop && npm test`
- `desktop/src-tauri`：Tauri 窗口。界面通过本机命令取数，不在作者的机器上留服务。
