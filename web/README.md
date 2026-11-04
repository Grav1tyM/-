# 对局瞄准筛选

一个小页面：用 Steam 好友代码在 [Statlocker](https://statlocker.gg/) 上找到 Deadlock 玩家，列出爆头率和命中率都**高于**你所设阈值的对局。

不需要 API key。查询走 Statlocker 网站自己用的公开接口（玩家搜索、资料、`full-recent` / `true` 对局列表）。这些接口在不带网站来源时会要求 key，本服务按网站页面的方式去请求。

## 运行

需要 Node.js 22+（内置 `fetch`）。

```bash
cd web
node server.mjs
```

浏览器打开 http://127.0.0.1:8787 。端口可用环境变量 `PORT` 改。

## Windows 便携包

不需要安装 Node。在本目录执行：

```bash
node scripts/build-windows-zip.mjs path/to/statlocker-filter-windows.zip
```

压缩包里是一个 `statlocker-filter` 文件夹。解压后双击 `打开筛选.bat`（中文名乱码时双击 `Start.bat`）。它会用自带的 Windows 版 Node 打开同一个页面，筛选规则不变。用完后关闭那个黑色窗口。

没有自己的好友代码时，可以用 Statlocker 文档里的公开示例 `9560260`（见 https://statlocker.gg/llms.txt ）。

## 阈值

单位是百分数。例如爆头率 `27`、命中率 `59` 表示只要两项都 **大于** 这两个数的对局，等于不算。

默认只扫描资料页的「最近完整记录」（最多约 200 场）。选「全部对局」会改用网站「加载全部」那一次请求。
