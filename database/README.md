# 数据库初始化说明

## 1. 数据库方案

本项目采用 **Code First** 方案，使用 Entity Framework Core 8.0 + SQL Server LocalDB。

- 数据库上下文：`src/LibrarySeatReservation.Web/Data/AppDbContext.cs`
- 迁移文件：`src/LibrarySeatReservation.Web/Migrations/`
- 种子数据：`src/LibrarySeatReservation.Web/Data/SeedData.cs`

---

## 2. 首次建库建表

项目启动时**自动完成**，无需手动执行 SQL。

```bash
cd src/LibrarySeatReservation.Web
dotnet run --urls http://localhost:5000
```

`Program.cs` 启动时自动执行：
1. `context.Database.Migrate()` — 应用所有迁移，建库建表
2. `SeedData.Initialize(context)` — 如果 Seats 表为空，插入种子数据

---

## 3. 重建数据库

如需清空重新开始：

```bash
cd src/LibrarySeatReservation.Web
dotnet ef database drop
dotnet run --urls http://localhost:5000
```

启动时会自动重新建库、建表、初始化种子数据。

---

## 4. 表结构

| 表名 | 主要字段 | 说明 |
|------|---------|------|
| Seats | Id, Name, Location, HasPower, Status, CreatedAt | 座位信息，Name 唯一索引 |
| Reservations | Id, SeatId, UserName, ReserveDate, TimeSlot, Status, CreatedAt | 预约记录，(SeatId, ReserveDate, TimeSlot) 唯一索引 |
| AdminUsers | Id, Username, Password, CreatedAt | 管理员账号，Username 唯一索引 |

---

## 5. 种子数据

首次启动时自动初始化（详见 [seed-data.md](seed-data.md)）：

| 表 | 数量 | 说明 |
|----|------|------|
| Seats | 20 条 | A/B/C/D 四区各 5 个座位，其中 3 个维护中 |
| AdminUsers | 1 条 | admin / 123456（PasswordHasher 哈希存储） |
| Reservations | 0 条 | 空表，运行后由用户创建 |

---

## 6. 连接字符串

默认配置（`appsettings.json`）：

```
Server=(localdb)\mssqllocaldb;Database=LibrarySeatReservationDb;Trusted_Connection=True;MultipleActiveResultSets=true
```

如需切换数据库，修改 `appsettings.json` 中的 `ConnectionStrings.DefaultConnection` 即可。

---

## 7. Docker 环境

```bash
cd src/LibrarySeatReservation.Web
docker-compose up -d
```

Docker 环境同样使用 LocalDB 连接串，需要宿主机安装 SQL Server LocalDB。
