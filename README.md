# 图书馆座位预约系统

> 高校图书馆高峰期座位紧张，学生到馆后经常找不到座位，或出现占座不用的情况。本项目开发一个轻量级座位预约系统，让学生可以提前在线预约座位，管理员可以管理座位资源和查看预约情况。

---

## 技术栈

| 层级 | 技术 | 版本 |
|------|------|------|
| 运行时 | .NET | 6.0 |
| Web 框架 | ASP.NET Core MVC | 6.0 |
| 视图引擎 | Razor | - |
| ORM | Entity Framework Core | 6.0 |
| 数据库 | SQL Server LocalDB | - |
| 前端样式 | Bootstrap | 5.x |
| 前端脚本 | jQuery | - |
| 密码加密 | PasswordHasher | ASP.NET Core 内置 |

---

## 目录结构

### 当前已存在

```
LibrarySeatSystem/
├── Controllers/                    # 控制器层
│   ├── HomeController.cs           # 用户首页
│   ├── SeatController.cs           # 座位列表、详情
│   ├── ReservationController.cs    # 预约提交、我的预约、取消、切换账号
│   └── AdminController.cs          # 管理员登录、座位管理、预约管理、统计
├── Models/
│   ├── Entities/                   # 实体类（Seat, Reservation, AdminUser）
│   └── Enums/                      # 枚举定义（SeatStatus, ReservationStatus）
├── Data/                           # 数据访问层
│   ├── AppDbContext.cs             # EF Core 数据库上下文
│   └── SeedData.cs                 # 种子数据（20座位 + 1管理员）
├── Views/                          # 视图层（9个页面）
│   ├── Home/Index.cshtml
│   ├── Seat/List.cshtml, Detail.cshtml
│   ├── Reservation/Create.cshtml, My.cshtml
│   ├── Admin/Login.cshtml, SeatIndex.cshtml, SeatCreate.cshtml,
│   │         SeatEdit.cshtml, ReservationIndex.cshtml, Statistics.cshtml
│   └── Shared/_Layout.cshtml, _SeatForm.cshtml
├── Helpers/PasswordHelper.cs       # PasswordHasher 封装
├── Filters/                        # 过滤器
│   ├── AdminAuthorizationFilter.cs # 管理员认证
│   └── GlobalExceptionFilter.cs    # 全局异常处理
├── Constants/TimeSlots.cs          # 时段常量（5个固定时段）
├── Migrations/                     # EF Core 迁移
├── wwwroot/                        # 静态资源（CSS/JS/lib）
├── Program.cs                      # 程序入口
├── appsettings.json                # 配置文件
├── LibrarySeatSystem.csproj        # 项目文件
├── Dockerfile                      # Docker 构建文件
├── docker-compose.yml              # Docker Compose 配置
└── .dockerignore                   # Docker 忽略文件
```

### 后续计划 / 待生成项

| 内容 | 说明 | 预计阶段 |
|------|------|----------|
| 单元测试项目 | `LibrarySeatSystem.Tests/` | 后续扩展 |
| Service 层 | `Services/` 目录（当前业务逻辑在 Controller 中） | 后续扩展 |
| ViewModel 层 | `Models/ViewModels/`（当前使用 ViewBag） | 后续扩展 |

---

## 运行前提

| 依赖 | 版本 | 说明 |
|------|------|------|
| .NET SDK | 6.0+ | `dotnet --version` 验证 |
| SQL Server LocalDB | 2019+ | `sqllocaldb info` 验证 |
| dotnet-ef | 6.0+ | `dotnet tool list -g` 验证 |

---

## 当前阶段

**阶段：开发准备与 Sprint 0**

本项目已完成全部代码开发与功能验证，当前处于文档体系完善阶段。详见 [docs/10-开发准备与Sprint0.md](docs/10-开发准备与Sprint0.md)。

---

## 快速开始

### 本地运行

```bash
cd LibrarySeatSystem
dotnet run --urls http://localhost:5000
```

- 用户端：http://localhost:5000
- 管理端：http://localhost:5000/Admin/Login

### Docker 运行

```bash
cd LibrarySeatSystem
docker-compose up -d
```

---

## 已实现范围

### 用户端（5页）

| 页面 | 路由 | 功能 |
|------|------|------|
| 用户首页 | `/Home/Index` | 欢迎语、统计数据、快速入口 |
| 座位列表 | `/Seat/List` | 分页展示20个座位 |
| 座位详情 | `/Seat/Detail/{id}` | 座位信息 + 5个时段状态 |
| 预约提交 | `/Reservation/Create/{seatId}` | 选择日期时段，冲突校验 |
| 我的预约 | `/Reservation/My` | 查看/取消当前账号预约 |

### 管理端（4页）

| 页面 | 路由 | 功能 |
|------|------|------|
| 管理员登录 | `/Admin/Login` | PasswordHasher 认证 |
| 座位管理 | `/Admin/SeatIndex` | 座位增删改 |
| 预约管理 | `/Admin/ReservationIndex` | 按日期/状态筛选 |
| 统计页 | `/Admin/Statistics` | 总预约数、热门座位TOP5、时段分布 |

---

## 数据库初始化

项目启动时自动执行：
1. `context.Database.Migrate()` — 应用迁移建库建表
2. `SeedData.Initialize(context)` — 初始化种子数据

---

## 演示账号

### 体验账号（学生端）

| 账号 | 角色 | 说明 |
|------|------|------|
| 张三 | 学生 | 默认体验账号 |
| 李四 | 学生 | 体验账号 |
| 王五 | 学生 | 体验账号 |

### 管理员账号

| 账号 | 密码 | 说明 |
|------|------|------|
| admin | 123456 | 管理员登录 |

---

## 已知限制

| 限制 | 说明 | 后续改进方向 |
|------|------|-------------|
| 体验账号硬编码 | 张三/李四/王五通过 Session 模拟 | 新增 User 表实现注册登录 |
| 时段固定5个值 | Morning1/Morning2/Afternoon1/Afternoon2/Evening | 可配置化 |
| 无签到功能 | Status=1（已签到）本期不使用 | 后续可扩展 |
| 无操作日志 | 课堂项目无审计需求 | 新增 Log 表 |
| 无并发控制 | 课堂项目不考虑 | 乐观锁/悲观锁 |
| 统计数据无缓存 | 实时计算，每次重新查询 | 引入 IMemoryCache 或 Redis |

---

## 文档索引

| 编号 | 文档 | 说明 |
|------|------|------|
| 01 | [项目立项单](docs/01-项目立项单.md) | 项目背景与目标 |
| 02 | [需求分析与MVP确认](docs/02-需求分析与MVP确认.md) | 需求范围 |
| 03 | [PRD-Lite](docs/03-PRD-Lite.md) | 产品需求文档 |
| 04 | [页面树与业务流程](docs/04-页面树与业务流程.md) | 页面清单与流程 |
| 05 | [页面卡与UI规范](docs/05-页面卡与UI规范.md) | UI 设计规范 |
| 06 | [静态原型与原型评审](docs/06-静态原型与原型评审.md) | 原型评审 |
| 07 | [系统设计说明](docs/07-系统设计说明.md) | 分层设计与工程落点 |
| 08 | [数据库设计](docs/08-数据库设计.md) | 表结构与索引 |
| 09 | [关键链路详细设计](docs/09-关键链路详细设计.md) | 7条链路详细设计 |
| 09d | [部署指南](docs/09-部署指南.md) | Docker 部署 |
| 10 | [开发准备与Sprint0](docs/10-开发准备与Sprint0.md) | 开发规范与Sprint规划 |
| - | [项目任务板与迭代记录](docs/项目任务板与迭代记录.md) | 任务跟踪 |
