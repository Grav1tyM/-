# 工业维修智能化零件分配平台（IndustrialMaintenance）

C# WinForms + SunnyUI + MySQL，分层：`MyUI` / `MyDAL` / `MyEntity`。

## 环境要求

- Windows + Visual Studio 2019/2022（需安装「.NET 桌面开发」）
- .NET Framework 4.7.2
- MySQL（库名 `IndustrialMaintenance`，脚本见 `Scripts/SQL脚本.txt`）
- NuGet 包：`SunnyUI` 3.9.8、`SunnyUI.Common` 3.9.8、`MySql.Data` 8.0.33

## 快速开始

1. 在 Navicat / MySQL 中执行 `Scripts/SQL脚本.txt`（若已导入可跳过）。
2. 用 VS 打开 `IndustrialMaintenance.sln`。
3. 右键解决方案 → **还原 NuGet 包**（若还原失败：对 MyUI / MyDAL 分别「管理 NuGet 包」安装上述包）。
4. 确认 `MyUI/App.config` 连接串（默认 `root` / `114514`，库 `IndustrialMaintenance`）。
5. 将 `MyUI` 设为启动项目，F5 运行。

## 默认演示账号

| 用户名 | 密码 | 角色 |
|--------|------|------|
| admin | admin123 | 系统管理员 |
| engineer | 123456 | 维修工程师 |
| store | 123456 | 仓管员 |

## 模块一览

- 登录（窗体标题与截图一致）
- 维修工单（报修 / 智能算料 / 完工归档）
- 领料分配（出库扣库存）
- 配件库存、配件分类、BOM 算料规则
- 维修历史、用户管理（管理员）

更完整说明见仓库外文档或 Context：`docs/industrial-maintenance.md`。
