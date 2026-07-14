# 图书馆座位预约系统

> 高校图书馆高峰期座位紧张，学生到馆后经常找不到座位，或出现占座不用的情况。本项目开发一个轻量级座位预约系统，让学生可以提前在线预约座位，管理员可以管理座位资源和查看预约情况。

---

## 技术栈

| 层级 | 技术 | 版本 |
|------|------|------|
| 运行时 | .NET | 8.0 |
| Web 框架 | ASP.NET Core MVC | 8.0 |
| 视图引擎 | Razor | - |
| ORM | Entity Framework Core | 8.0 |
| 数据库 | SQL Server LocalDB | - |
| 前端样式 | Bootstrap | 5.x |
| 前端脚本 | jQuery | - |
| 密码加密 | PasswordHasher | ASP.NET Core 内置 |
| 缓存 | IMemoryCache | 5分钟过期 |
| 容器化 | Docker | 多阶段构建 |

---

## 项目结构

### 新版项目（src/LibrarySeatReservation.Web）

```
src/LibrarySeatReservation.Web/
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
├── DataAccess/                     # 数据访问接口与实现
│   ├── ISeatRepository.cs / SeatRepository.cs
│   ├── IReservationRepository.cs / ReservationRepository.cs
│   └── IAdminUserRepository.cs / AdminUserRepository.cs
├── Services/                       # 业务逻辑层
│   ├── ISeatService.cs / SeatService.cs
│   ├── IReservationService.cs / ReservationService.cs
│   └── IAdminService.cs / AdminService.cs
├── Helpers/
│   └── PasswordHelper.cs           # PasswordHasher 封装
├── Constants/
│   └── TimeSlots.cs                # 时段常量（5个固定时段）
├── Views/                          # 视图层（10页）
│   ├── Home/Index.cshtml
│   ├── Seat/List.cshtml, Detail.cshtml
│   ├── Reservation/Create.cshtml, My.cshtml
│   ├── Admin/Login.cshtml, SeatIndex.cshtml, SeatCreate.cshtml,
│   │   SeatEdit.cshtml, ReservationIndex.cshtml, Statistics.cshtml
│   └── Shared/_Layout.cshtml, _AdminLayout.cshtml
├── Migrations/                     # EF Core 迁移
├── wwwroot/                        # 静态资源（CSS/JS/lib）
├── Program.cs                      # 程序入口（DI + Session + 自动迁移）
├── appsettings.json                # 配置文件
├── Dockerfile                      # Docker 多阶段构建
├── docker-compose.yml              # Docker Compose 配置
└── LibrarySeatReservation.Web.csproj
```

---

## 功能清单

### 用户端

| 页面 | 路由 | 功能 |
|------|------|------|
| 首页 | `GET /` | 欢迎语、快捷入口 |
| 座位列表 | `GET /Seat/List?page=1` | 分页展示20个座位 |
| 座位详情 | `GET /Seat/Detail/{id}` | 座位信息 + 5时段状态 |
| 预约提交 | `GET/POST /Reservation/Create?seatId=&timeSlot=` | 选择日期时段，冲突校验 |
| 我的预约 | `GET /Reservation/My` | 当前用户预约记录 |
| 取消预约 | `POST /Reservation/Cancel/{id}` | 仅限 Status=0 |

### 管理端

| 页面 | 路由 | 功能 |
|------|------|------|
| 管理员登录 | `GET/POST /Admin/Login` | PasswordHasher 验证 |
| 座位管理 | `GET /Admin/SeatIndex` | 座位列表 + 编辑/删除（编号唯一性校验） |
| 新增座位 | `GET/POST /Admin/SeatCreate` | 创建新座位（编号重复提示） |
| 编辑座位 | `GET/POST /Admin/SeatEdit/{id}` | 修改座位信息 |
| 删除座位 | `POST /Admin/SeatDelete/{id}` | 删除座位（有预约记录时阻止） |
| 预约管理 | `GET /Admin/ReservationIndex` | 按日期/状态筛选，数据库级分页 |
| 统计 | `GET /Admin/Statistics` | 总数/今日/热门TOP5(显示名称)/时段分布（缓存5分钟） |

---

## 运行前提

| 依赖 | 版本 | 说明 |
|------|------|------|
| .NET SDK | 8.0+ | `dotnet --version` 验证 |
| SQL Server LocalDB | 2019+ | `sqllocaldb info` 验证 |
| dotnet-ef | 8.0+ | `dotnet tool install --global dotnet-ef --version 8.0.0` |

---

## 快速开始

### 本地运行

```bash
cd src/LibrarySeatReservation.Web
dotnet run --urls http://localhost:5000
```

- 用户端：http://localhost:5000
- 管理端：http://localhost:5000/Admin/Login

### 数据库初始化

项目启动时自动执行：
1. `context.Database.Migrate()` — 应用迁移建库建表
2. `SeedData.Initialize(context)` — 初始化种子数据（如表为空）

如需重建数据库：
```bash
cd src/LibrarySeatReservation.Web
dotnet ef database drop
dotnet ef database update
```

### Docker 运行

```bash
cd src/LibrarySeatReservation.Web
docker-compose up -d
```

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
| admin | 123456 | 管理员登录（密码经 PasswordHasher 哈希存储） |

---

## 数据库

| 表名 | 字段 | 种子数据 |
|------|------|---------|
| Seats | Id, Name, Location, HasPower, Status, CreatedAt | 20条（A/B/C/D区各5个） |
| Reservations | Id, SeatId, UserName, ReserveDate, TimeSlot, Status, CreatedAt | 空 |
| AdminUsers | Id, Username, Password, CreatedAt | 1条（admin/123456） |

---

## 开发阶段

| Sprint | 目标 | 状态 |
|--------|------|------|
| Sprint 0 | 骨架搭建（.NET 8, EF Core 8, DI, Session） | ✅ 完成 |
| Sprint 1 | 用户端核心链路（座位→详情→预约→取消） | ✅ 完成 |
| Sprint 2 | 管理端核心链路（登录→座位CRUD→预约管理→统计） | ✅ 完成 |
| Sprint 3 | 性能优化（AsNoTracking, MemoryCache）+ Docker | ✅ 完成 |
| Sprint 4 | 功能验证 + 文档审计 + 最终交付 | ✅ 完成 |
| 功能完善 | 审计缺口补全、异常处理、UI 样式、空状态、响应式 | ✅ 完成 |
| 联调测试 | 全链路联调、代码审查、缺陷闭环 | ✅ 完成 |

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
| 10 | [开发准备与Sprint0](docs/10-开发准备与Sprint0.md) | 开发规范与Sprint规划 |
| 11 | [开发前一致性总审计](docs/11-开发前一致性总审计.md) | 文档一致性审计 |
| 12 | [开发起步与骨架记录](docs/12-开发起步与骨架记录.md) | Sprint 0 骨架记录 |
| 13 | [用户端主链路开发记录](docs/13-用户端主链路开发记录.md) | Sprint 1 用户端验证 |
| 14 | [管理端与权限开发记录](docs/14-管理端与权限开发记录.md) | Sprint 2 管理端验证 |
| 15 | [功能完善与体验优化记录](docs/15-功能完善与体验优化记录.md) | 功能完善与体验优化 |
| 16 | [联调测试与缺陷闭环记录](docs/16-联调测试与缺陷闭环记录.md) | 联调测试与缺陷闭环 |
| 17 | [最终交付记录](docs/17-最终交付记录.md) | 最终交付 |
| - | [项目任务板与迭代记录](docs/项目任务板与迭代记录.md) | 任务跟踪 |
