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
| 数据库 | SQL Server LocalDB | 2019+ |
| 前端 | Bootstrap 5 + jQuery | - |
| 密码加密 | PasswordHasher | ASP.NET Core 内置 |
| 缓存 | IMemoryCache | 5分钟过期 |
| 自动化测试 | Playwright | 1.52.0 |
| 容器化 | Docker | 多阶段构建 |

---

## 快速开始

### 环境要求

| 依赖 | 版本 | 验证命令 |
|------|------|----------|
| .NET SDK | 8.0+ | `dotnet --version` |
| SQL Server LocalDB | 2019+ | `sqllocaldb info` |
| Node.js（仅测试） | 18+ | `node -v` |

### 本地运行

```bash
cd src/LibrarySeatReservation.Web
dotnet run --urls http://localhost:5000
```

- 用户端首页：http://localhost:5000
- 管理端登录：http://localhost:5000/Admin/Login

### Docker 运行

```bash
cd src/LibrarySeatReservation.Web
docker-compose up -d
```

### 运行自动化测试

```bash
npm install
npx playwright install
npx playwright test
```

---

## 演示账号

### 体验账号（学生端）

| 账号 | 角色 | 说明 |
|------|------|------|
| 张三 | 学生 | 默认体验账号 |
| 李四 | 学生 | 导航栏切换 |
| 王五 | 学生 | 导航栏切换 |

体验账号通过 Session 切换，无需密码，导航栏下拉选择即可。

### 管理员账号

| 用户名 | 密码 | 说明 |
|--------|------|------|
| admin | 123456 | 管理端登录（密码经 PasswordHasher 哈希存储） |

---

## 功能清单

### 用户端（5 页）

| 页面 | 路由 | 功能 |
|------|------|------|
| 首页 | `GET /` | 欢迎语、当前用户、今日日期、快捷入口 |
| 座位列表 | `GET /Seat/List?page=1` | 分页展示 20 个座位，含电源/位置/状态 |
| 座位详情 | `GET /Seat/Detail/{id}` | 座位信息 + 5 个时段预约状态 |
| 预约提交 | `GET/POST /Reservation/Create?seatId=&timeSlot=` | 选择日期时段，冲突校验 |
| 我的预约 | `GET /Reservation/My` | 当前用户预约记录，支持取消 |

### 管理端（5 页）

| 页面 | 路由 | 功能 |
|------|------|------|
| 管理员登录 | `GET/POST /Admin/Login` | PasswordHasher 验证 |
| 座位管理 | `GET /Admin/SeatIndex` | 座位列表 + 编辑/删除 |
| 新增/编辑座位 | `GET/POST /Admin/SeatCreate`, `/Admin/SeatEdit/{id}` | 座位 CRUD |
| 预约管理 | `GET /Admin/ReservationIndex` | 按日期/状态筛选，分页 |
| 统计 | `GET /Admin/Statistics` | 总数/今日/热门 TOP5/时段分布 |

---

## 项目结构

```
├── src/LibrarySeatReservation.Web/    # 主项目
│   ├── Controllers/                   # 控制器（4 个）
│   │   ├── HomeController.cs          # 用户首页
│   │   ├── SeatController.cs          # 座位列表、详情
│   │   ├── ReservationController.cs   # 预约提交、我的预约、取消、切换账号
│   │   └── AdminController.cs         # 管理员登录、座位管理、预约管理、统计
│   ├── Models/Entities/               # 实体类（Seat, Reservation, AdminUser）
│   ├── Data/                          # DbContext + 种子数据
│   ├── DataAccess/                    # Repository 接口与实现
│   ├── Services/                      # Service 接口与实现
│   ├── Views/                         # Razor 视图（10 页）
│   ├── Constants/                     # 时段常量
│   ├── Helpers/                       # PasswordHasher 封装
│   ├── Migrations/                    # EF Core 迁移
│   ├── Program.cs                     # 入口（DI + Session + 自动迁移）
│   ├── appsettings.json               # 数据库连接串
│   ├── Dockerfile                     # Docker 多阶段构建
│   └── docker-compose.yml             # Docker Compose 配置
├── database/                          # 数据库初始化说明
├── docs/                              # 项目文档（17 篇 + 任务板）
├── tests/                             # Playwright 自动化测试
├── prototype/                         # 静态原型
├── LibrarySeatReservation.slnx        # .NET 8 解决方案
├── playwright.config.ts               # Playwright 配置
├── package.json                       # Node.js 依赖（测试用）
└── README.md                          # 本文件
```

---

## 数据库

### 初始化方式

采用 **Code First**，项目启动时自动完成：

1. `context.Database.Migrate()` — 应用迁移建库建表
2. `SeedData.Initialize(context)` — 插入种子数据（如表为空）

无需手动执行 SQL。如需重建：

```bash
cd src/LibrarySeatReservation.Web
dotnet ef database drop
dotnet run --urls http://localhost:5000
```

详见 [database/README.md](database/README.md)。

### 种子数据

| 表 | 数量 | 说明 |
|----|------|------|
| Seats | 20 条 | A/B/C/D 四区各 5 个，其中 3 个维护中 |
| AdminUsers | 1 条 | admin / 123456 |
| Reservations | 0 条 | 运行后由用户创建 |

详见 [database/seed-data.md](database/seed-data.md)。

---

## 已知限制

| 编号 | 限制 | 说明 |
|------|------|------|
| L-01 | 无单元测试项目 | 课程演示系统，未引入 xUnit/NUnit |
| L-02 | 体验账号切换无鉴权 | 设计如此，演示用途 |
| L-03 | 统计缓存无主动失效 | 5分钟 TTL 过期自动刷新 |
| L-04 | 无邮件/短信通知 | 本期范围外 |
| L-05 | 无数据导出功能 | 本期范围外 |
| L-06 | 取消预约后无法重新预约同时段 | 唯一索引限制，取消记录仍占时段 |
| L-07 | 预约日期缺服务端过去日期校验 | 客户端 min 属性限制，低风险 |

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
| 17 | [交付说明与项目复盘](docs/17-交付说明与项目复盘.md) | 最终交付与复盘 |
| - | [项目任务板与迭代记录](docs/项目任务板与迭代记录.md) | 任务跟踪 |
