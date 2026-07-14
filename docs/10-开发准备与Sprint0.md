# 开发准备与 Sprint 0

> 文档类型：开发规范、Sprint 规划与里程碑定义
> 输入来源：07-系统设计说明.md、08-数据库设计.md、09-关键链路详细设计.md
> 输出目标：指导开发执行，确保节奏可控

---

## 1. 仓库结构

### 1.1 仓库根目录

```
LibrarySeatSystem/
├── LibrarySeatSystem/          # 主项目目录（ASP.NET Core MVC 应用）
├── docs/                       # 文档目录
│   ├── 01-项目立项单.md
│   ├── 02-需求分析与MVP确认.md
│   ├── 03-PRD-Lite.md
│   ├── 04-页面树与业务流程.md
│   ├── 05-页面卡与UI规范.md
│   ├── 06-静态原型与原型评审.md
│   ├── 07-系统设计说明.md
│   ├── 08-数据库设计.md
│   ├── 09-关键链路详细设计.md
│   ├── 09-部署指南.md
│   ├── 10-开发准备与Sprint0.md  ← 本文档
│   ├── 项目任务板与迭代记录.md
│   └── prototypes/              # 静态原型 HTML
├── README.md                    # 项目说明（根目录）
└── .gitignore
```

### 1.2 项目目录

详见 [README.md](../README.md) 目录结构章节。

---

## 2. 分支策略

```
main                    ← 生产分支，仅通过 PR 合入
├── develop             ← 开发主干，Sprint 交付合入
│   ├── feat/sprint-1-user-core     ← Sprint 1 用户端功能
│   ├── feat/sprint-2-admin-core    ← Sprint 2 管理端功能
│   ├── feat/sprint-3-optimize      ← Sprint 3 优化部署
│   └── feat/sprint-4-final         ← Sprint 4 收尾审计
└── hotfix/*            ← 紧急修复
```

**分支规则**：

| 规则 | 说明 |
|------|------|
| main 只读 | 所有变更通过 PR 合入 main |
| develop 是主干 | 日常开发在 develop 及其子分支 |
| 功能分支命名 | `feat/sprint-{N}-{简述}` |
| 合并方式 | 功能分支 → develop → main |
| 删除策略 | 合并后删除功能分支 |

---

## 3. 提交规范

### 3.1 提交信息格式

```
{类型}({范围}): {描述}

类型：
  feat     新功能
  fix      修复
  docs     文档
  style    格式（不影响逻辑）
  refactor 重构
  test     测试
  chore    构建/工具
  perf     性能优化

范围（可选）：
  seat     座位模块
  reserv   预约模块
  admin    管理模块
  home     首页模块
  db       数据库
  auth     认证授权
  ui       界面/布局

示例：
feat(seat): 实现座位列表分页
fix(reserv): 修复预约冲突校验逻辑
docs: 更新数据库设计文档
chore: 配置 EF Core 迁移
```

### 3.2 提交粒度

| 粒度 | 说明 | 示例 |
|------|------|------|
| 功能级 | 一个完整功能点 | "实现座位详情页时段状态展示" |
| 修复级 | 一个 Bug 修复 | "修复取消预约权限校验" |
| 文档级 | 一次文档更新 | "更新关键链路设计" |

---

## 4. Sprint 0 目标

### 4.1 目标定义

> **Sprint 0 目标**：完成开发准备，确保项目可编译、可运行、可追踪。

### 4.2 Sprint 0 任务清单

| 编号 | 任务 | 验收标准 | 关联文档 | 状态 |
|------|------|----------|----------|------|
| T12-01 | 创建解决方案 .sln | `dotnet new sln` 成功 | 12-开发环境配置 | ✅ |
| T12-02 | 创建 Web 项目 .csproj | `dotnet new mvc` 成功，TargetFramework=net6.0 | 12-开发环境配置 | ✅ |
| T12-03 | 配置 EF Core + SQL Server LocalDB | csproj 包含 EF Core SqlServer 6.0 | 08-数据库设计 | ✅ |
| T12-04 | 创建 AppDbContext | DbSet<Seat/Reservation/AdminUser> 声明完整 | 08-数据库设计 | ✅ |
| T12-05 | 创建实体类（Seat, Reservation, AdminUser） | 字段与 08-数据库设计 §4 一致 | 08-数据库设计 | ✅ |
| T12-06 | 创建枚举定义（SeatStatus, ReservationStatus） | 枚举值与 08-数据库设计 §6 一致 | 08-数据库设计 | ✅ |
| T12-07 | 创建时段常量（TimeSlots） | 5个固定时段值 | 08-数据库设计 §5.3 | ✅ |
| T12-08 | 创建 EF Core 迁移 | `dotnet ef migrations add InitialCreate` 成功 | 08-数据库设计 | ✅ |
| T12-09 | 首次 dotnet build | 0 errors | - | ✅ |
| T12-10 | 创建种子数据（SeedData） | 20座位 + 1管理员 | 08-数据库设计 §4 | ✅ |
| T12-11 | 首次 dotnet run | http://localhost:5000 可访问 | - | ✅ |
| T12-12 | 验证数据库初始化 | 启动后数据库表和种子数据存在 | - | ✅ |
| T12-13 | 创建 PasswordHelper | PasswordHasher 封装完成 | 07-系统设计 §2 | ✅ |
| T12-14 | 创建 AdminAuthorizationFilter | 未登录跳转登录页，AllowAnonymous 跳过 | 09-关键链路 §5 | ✅ |
| T12-15 | 创建 GlobalExceptionFilter | 异常时返回 Error 视图 | 07-系统设计 §8 | ✅ |

### 4.3 Sprint 0 交付物

- [x] `LibrarySeatSystem.sln`
- [x] `LibrarySeatSystem.csproj`（含 EF Core 依赖）
- [x] 实体类、枚举、常量
- [x] AppDbContext + 迁移
- [x] 种子数据
- [x] 首次 build + run 成功
- [x] 开发规范文档（本文档）
- [x] 项目任务板

---

## 5. Sprint 1-4 主 Sprint 粗计划

> **重要**：每个主 Sprint 允许多轮推进。一轮指一次"编码→提交→验证"的完整循环。当一轮无法完成 Sprint 全部目标时，可追加第二轮、第三轮，直到该 Sprint 阶段最低完成线达标后方可进入下一阶段。

### 5.1 Sprint 1：用户端核心链路

| 属性 | 内容 |
|------|------|
| **Sprint 目标** | 实现学生预约座位完整闭环（L1 + L2 + L3） |
| **阶段最低完成线** | 座位列表→详情→预约提交→我的预约 可走通 |
| **预计轮次** | 1-2 轮 |
| **关联链路** | L1 学生预约座位、L2 学生取消预约、L3 切换体验账号 |
| **关联页面** | U1 首页、U2 座位列表、U3 座位详情、U4 预约提交、U5 我的预约 |

**任务范围**：

| 编号 | 任务 | 对应链路 | 优先级 |
|------|------|----------|--------|
| T13-01 | 实现全局布局 _Layout.cshtml（导航栏 + 体验账号切换） | - | P0 |
| T13-02 | 实现用户首页（HomeController.Index） | - | P0 |
| T13-03 | 实现座位列表页（SeatController.List，分页） | L1 | P0 |
| T13-04 | 实现座位详情页（SeatController.Detail，时段状态） | L1 | P0 |
| T13-05 | 实现预约提交页 GET（ReservationController.Create） | L1 | P0 |
| T13-06 | 实现预约提交 POST + 冲突校验 | L1 | P0 |
| T13-07 | 实现我的预约页（ReservationController.My） | L1/L2 | P0 |
| T13-08 | 实现取消预约 POST（ReservationController.Cancel） | L2 | P1 |
| T13-09 | 实现体验账号切换 POST（ReservationController.SwitchUser） | L3 | P0 |

**最低完成线验收**：
- [ ] 座位列表显示20条数据，分页正常
- [ ] 座位详情显示5个时段状态
- [ ] 预约提交成功后跳转我的预约
- [ ] 冲突时提示"该时段已被预约"
- [ ] 我的预约只显示当前账号的记录
- [ ] 取消预约后状态变为"已取消"
- [ ] 切换账号后数据联动

### 5.2 Sprint 2：管理端核心链路

| 属性 | 内容 |
|------|------|
| **Sprint 目标** | 实现管理员登录、座位管理、预约查看、统计（L4 + L5 + L6 + L7） |
| **阶段最低完成线** | 管理员登录→座位增删改→预约列表 可走通 |
| **预计轮次** | 1-2 轮 |
| **关联链路** | L4 管理员登录、L5 管理员座位管理、L6 预约查看、L7 统计 |
| **关联页面** | A1 登录、A2 座位管理、A3 预约管理、A4 统计 |

**任务范围**：

| 编号 | 任务 | 对应链路 | 优先级 |
|------|------|----------|--------|
| T14-01 | 实现管理员登录页 GET（AdminController.Login） | L4 | P0 |
| T14-02 | 实现管理员登录 POST + PasswordHasher | L4 | P0 |
| T14-03 | 实现管理员登出 | L4 | P0 |
| T14-04 | 实现座位管理页列表（AdminController.SeatIndex） | L5 | P0 |
| T14-05 | 实现座位新增 GET + POST | L5 | P0 |
| T14-06 | 实现座位编辑 GET + POST | L5 | P0 |
| T14-07 | 实现座位删除 POST | L5 | P1 |
| T14-08 | 实现预约管理页（筛选 + 分页） | L6 | P1 |
| T14-09 | 实现统计页（聚合查询） | L7 | P1 |

**最低完成线验收**：
- [ ] admin/123456 登录成功
- [ ] 未登录访问管理页自动跳转登录页
- [ ] 座位管理页显示所有座位
- [ ] 新增/编辑/删除座位功能正常
- [ ] 预约管理页可按日期/状态筛选
- [ ] 统计页数据正确

### 5.3 Sprint 3：优化与部署

| 属性 | 内容 |
|------|------|
| **Sprint 目标** | 性能优化、Docker 部署、功能测试 |
| **阶段最低完成线** | Docker 可构建运行，核心功能验证通过 |
| **预计轮次** | 1 轮 |
| **关联文档** | 09-部署指南 |

**任务范围**：

| 编号 | 任务 | 优先级 |
|------|------|--------|
| T15-01 | 性能优化（AsNoTracking、统计查询合并） | P1 |
| T15-02 | 引入 IMemoryCache 缓存统计数据 | P1 |
| T15-03 | 创建 Dockerfile | P1 |
| T15-04 | 创建 docker-compose.yml | P1 |
| T15-05 | 验证 Docker 构建与运行 | P0 |
| T15-06 | 功能测试（81项全通过） | P0 |

**最低完成线验收**：
- [ ] Docker 可正常构建和运行
- [ ] 所有功能可正常使用
- [ ] 功能测试报告完成

### 5.4 Sprint 4：收尾与审计

| 属性 | 内容 |
|------|------|
| **Sprint 目标** | 文档审计、代码审计、最终交付 |
| **阶段最低完成线** | 全部文档审计通过，代码无 critical 问题 |
| **预计轮次** | 1 轮 |

**任务范围**：

| 编号 | 任务 | 优先级 |
|------|------|--------|
| T16-01 | 文档一致性总审计 | P0 |
| T16-02 | 代码审计修复 | P0 |
| T16-03 | 更新 README.md | P1 |
| T16-04 | 最终 git commit + tag | P0 |

**最低完成线验收**：
- [ ] 全部文档审计通过
- [ ] 代码无 critical/high 问题
- [ ] README 反映最终状态

---

## 6. 里程碑节点

| 里程碑 | 对应 Sprint | 最低交付物 | 判定标准 |
|--------|-------------|------------|----------|
| M1：项目可运行 | Sprint 0 | .sln + .csproj + 迁移 + 种子数据 | `dotnet run` 可访问首页 |
| M2：用户端闭环 | Sprint 1 | 用户端5页可完整走通 | 预约→查看→取消 全流程通过 |
| M3：管理端闭环 | Sprint 2 | 管理端4页可完整走通 | 登录→管理→统计 全流程通过 |
| M4：可部署交付 | Sprint 3+4 | Docker 部署 + 文档审计 | Docker 运行正常，文档一致 |

---

## 7. 默认补足项 / 当前假设

### 7.1 从前序文档明确继承的内容

| 项目 | 来源 | 说明 |
|------|------|------|
| 4个Controller | 07-系统设计 §5.1 | Home, Seat, Reservation, Admin |
| 17个Action | 07-系统设计 §6 | 用户端8个 + 管理端9个 |
| 7条关键链路 | 09-关键链路 §1.1 | L1-L7 |
| 3张核心表 | 08-数据库设计 §2 | Seat, Reservation, AdminUser |
| 20条种子座位 | 08-数据库设计 §4.1 | A区5+B区5+C区5+D区5 |
| 1条种子管理员 | 08-数据库设计 §4.3 | admin / 123456 |
| 5个固定时段 | 08-数据库设计 §5.3 | Morning1/2, Afternoon1/2, Evening |

### 7.2 当前假设与补充说明

| 假设/补充 | 说明 |
|-----------|------|
| 本期不使用 Service 层 | 业务逻辑直接写在 Controller 中 |
| 本期不使用 ViewModel | 使用 ViewBag 传递数据 |
| 体验账号硬编码 | 张三/李四/王五，不建用户表 |
| 分页固定10条 | 不支持自定义每页条数 |
| 编辑座位不修改 CreatedAt | 只更新 Name, Location, HasPower, Status |
| 统计数据实时计算 | 无缓存，每次重新查询 |

---

**开发准备完成，下一步：进入 Sprint 1 用户端核心链路开发。**
