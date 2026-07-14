# 图书馆座位预约系统

高校图书馆高峰期座位紧张，学生到馆后经常找不到座位，或出现占座不用的情况。本项目开发一个轻量级座位预约系统，让学生可以提前在线预约座位，管理员可以管理座位资源和查看预约情况。

## 技术栈

- ASP.NET Core MVC (.NET 6)
- Razor 视图引擎
- SQL Server LocalDB
- Entity Framework Core 6
- Bootstrap 5.x
- jQuery

## 项目结构

```
LibrarySeatSystem/
├── Controllers/           # 控制器层
├── Models/                # 模型层
│   ├── Entities/          # 实体类
│   └── Enums/             # 枚举定义
├── Data/                  # 数据访问层
├── Views/                 # 视图层
├── Helpers/               # 辅助类
├── Filters/               # 过滤器
├── Constants/             # 常量定义
└── wwwroot/               # 静态资源
```

## 快速开始

### 方式一：本地运行

```bash
cd LibrarySeatSystem
dotnet run --urls http://localhost:5000
```

访问：
- 用户端：http://localhost:5000
- 管理端：http://localhost:5000/Admin/Login
  - 账号：admin
  - 密码：123456

### 方式二：Docker 运行

```bash
cd LibrarySeatSystem
docker-compose up -d
```

## 功能说明

### 用户端

- 首页：系统入口，提供快速导航
- 座位列表：浏览所有座位，支持分页
- 座位详情：查看座位信息和时段状态
- 预约提交：选择日期和时段提交预约
- 我的预约：查看和取消预约记录
- 体验账号切换：张三/李四/王五

### 管理端

- 管理员登录：用户名密码认证
- 座位管理：座位信息的增删改
- 预约管理：查看所有预约，支持筛选
- 统计页：总预约数、热门座位、时段分布

## 体验账号

| 账号 | 角色 | 说明 |
|------|------|------|
| 张三 | 学生 | 默认体验账号 |
| 李四 | 学生 | 体验账号 |
| 王五 | 学生 | 体验账号 |
| admin | 管理员 | 登录后台 |

## 数据库

- 类型：SQL Server LocalDB
- 表：Seat（20条种子数据）、Reservation、AdminUser（1条种子数据）
- 迁移：EF Core Migrations

## 文档

- [项目立项单](docs/01-项目立项单.md)
- [需求分析与MVP确认](docs/02-需求分析与MVP确认.md)
- [PRD-Lite](docs/03-PRD-Lite.md)
- [页面树与业务流程](docs/04-页面树与业务流程.md)
- [系统设计说明](docs/07-系统设计说明.md)
- [数据库设计](docs/08-数据库设计.md)
- [部署指南](docs/09-部署指南.md)
