# 工业维修智能化零件分配平台（IndustrialMaintenance）

C# WinForms + SunnyUI + MySQL，分层：`MyUI` / `MyDAL` / `MyEntity`。

## 本机打开路径（Agent Store，后续更新都在这里）

```
%LOCALAPPDATA%\Cursor\AgentStores\cursor_agent_stores\bc-b42c440a-6d8f-5a94-83fe-bf1ca772db28\files\docs\IndustrialMaintenance\IndustrialMaintenance.sln
```

展开一般为：

```
C:\Users\Gravity\AppData\Local\Cursor\AgentStores\cursor_agent_stores\bc-b42c440a-6d8f-5a94-83fe-bf1ca772db28\files\docs\IndustrialMaintenance\IndustrialMaintenance.sln
```

## 运行

1. VS2022 打开上述 `.sln`
2. 生成 → 重新生成解决方案（DLL 已内置在 `MyDAL\` / `MyUI\`，一般无需 NuGet）
3. 启动项目选 **MyUI** → F5
4. 登录 `admin` / `admin123`（MySQL：`root` / `114514`，库 `IndustrialMaintenance`）

## 环境

- .NET Framework 4.7.2
- 内置：`MySql.Data.dll`、`SunnyUI.dll`、`SunnyUI.Common.dll`
