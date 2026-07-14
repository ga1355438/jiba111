# 数据库设计与API定义

> 文档类型：数据库设计与API定义文档
> 输入来源：03-PRD-Lite.md、04-页面树与业务流程.md
> 输出目标：进入下一步"开发任务分解"

---

## 1. 数据库设计

### 1.1 ER关系图

```
┌─────────────────────┐         ┌─────────────────────┐
│       Seat          │         │    Reservation      │
├─────────────────────┤         ├─────────────────────┤
│ PK  Id       (int)  │◄───────FK│ SeatId    (int)    │
│     Name     (str)  │         │ PK  Id       (int)  │
│     Location (str)  │         │     UserName (str)  │
│     HasPower (bool) │         │     ReserveDate (date)│
│     Status   (int)  │         │     TimeSlot  (str) │
│     CreatedAt (datetime)│    │     Status    (int) │
└─────────────────────┘         │     CreatedAt (datetime)│
                                └─────────────────────┘
                                           │
                                           │
                                ┌─────────────────────┐
                                │     AdminUser       │
                                ├─────────────────────┤
                                │ PK  Id       (int)  │
                                │     Username (str)  │
                                │     Password (str)  │
                                └─────────────────────┘
```

### 1.2 数据表设计

---

#### 表 1：Seat（座位表）

| 字段名 | 数据类型 | 约束 | 默认值 | 说明 |
|--------|----------|------|--------|------|
| Id | int | PK, 自增, NOT NULL | - | 主键 |
| Name | nvarchar(50) | UNIQUE, NOT NULL | - | 座位编号，如"A-01" |
| Location | nvarchar(100) | NOT NULL | - | 位置描述 |
| HasPower | bit | NOT NULL | 0 | 是否有电源 |
| Status | int | NOT NULL | 0 | 0=可用, 1=维护中 |
| CreatedAt | datetime | NOT NULL | GETDATE() | 创建时间 |

**索引**：
| 索引名 | 字段 | 类型 | 说明 |
|--------|------|------|------|
| IX_Seat_Name | Name | UNIQUE | 座位编号唯一 |
| IX_Seat_Status | Status | NON-UNIQUE | 按状态查询 |

**种子数据**：
```sql
INSERT INTO Seat (Name, Location, HasPower, Status, CreatedAt) VALUES
('A-01', '一楼东侧', 1, 0, GETDATE()),
('A-02', '一楼东侧', 0, 0, GETDATE()),
('A-03', '一楼东侧', 1, 0, GETDATE()),
('A-04', '一楼西侧', 0, 0, GETDATE()),
('A-05', '一楼西侧', 1, 0, GETDATE()),
('B-01', '二楼东侧', 1, 0, GETDATE()),
('B-02', '二楼东侧', 0, 0, GETDATE()),
('B-03', '二楼东侧', 1, 0, GETDATE()),
('B-04', '二楼西侧', 0, 0, GETDATE()),
('B-05', '二楼西侧', 1, 0, GETDATE()),
('C-01', '三楼东侧', 1, 0, GETDATE()),
('C-02', '三楼东侧', 0, 0, GETDATE()),
('C-03', '三楼东侧', 1, 0, GETDATE()),
('C-04', '三楼西侧', 1, 0, GETDATE()),
('C-05', '三楼西侧', 0, 0, GETDATE()),
('D-01', '四楼东侧', 1, 0, GETDATE()),
('D-02', '四楼东侧', 0, 0, GETDATE()),
('D-03', '四楼西侧', 1, 0, GETDATE()),
('D-04', '四楼西侧', 0, 0, GETDATE()),
('D-05', '四楼西侧', 1, 1, GETDATE()); -- 维护中
```

---

#### 表 2：Reservation（预约表）

| 字段名 | 数据类型 | 约束 | 默认值 | 说明 |
|--------|----------|------|--------|------|
| Id | int | PK, 自增, NOT NULL | - | 主键 |
| SeatId | int | FK, NOT NULL | - | 关联 Seat 表 |
| UserName | nvarchar(50) | NOT NULL | - | 预约人姓名 |
| ReserveDate | date | NOT NULL | - | 预约日期 |
| TimeSlot | nvarchar(20) | NOT NULL | - | 时段 |
| Status | int | NOT NULL | 0 | 0=已预约, 1=已签到, 2=已取消, 3=已过期 |
| CreatedAt | datetime | NOT NULL | GETDATE() | 创建时间 |

**约束**：
| 约束名 | 类型 | 字段 | 说明 |
|--------|------|------|------|
| FK_Reservation_Seat | 外键 | SeatId | 关联 Seat.Id |
| UQ_Reservation | 唯一约束 | SeatId, ReserveDate, TimeSlot, Status | 同一座位同时段只能有一个有效预约 |

**索引**：
| 索引名 | 字段 | 类型 | 说明 |
|--------|------|------|------|
| IX_Reservation_SeatId | SeatId | NON-UNIQUE | 按座位查询 |
| IX_Reservation_UserName | UserName | NON-UNIQUE | 按用户查询 |
| IX_Reservation_ReserveDate | ReserveDate | NON-UNIQUE | 按日期查询 |
| IX_Reservation_Status | Status | NON-UNIQUE | 按状态查询 |

**时段枚举值**：
| 值 | 说明 |
|------|------|
| 08:00-10:00 | 上午第一段 |
| 10:00-12:00 | 上午第二段 |
| 14:00-16:00 | 下午第一段 |
| 16:00-18:00 | 下午第二段 |
| 19:00-21:00 | 晚上 |

**状态枚举值**：
| 值 | 说明 |
|------|------|
| 0 | 已预约 |
| 1 | 已签到（本期不用） |
| 2 | 已取消 |
| 3 | 已过期 |

---

#### 表 3：AdminUser（管理员表）

| 字段名 | 数据类型 | 约束 | 默认值 | 说明 |
|--------|----------|------|--------|------|
| Id | int | PK, 自增, NOT NULL | - | 主键 |
| Username | nvarchar(50) | UNIQUE, NOT NULL | - | 登录名 |
| Password | nvarchar(100) | NOT NULL | - | 密码（MD5 哈希） |

**种子数据**：
```sql
INSERT INTO AdminUser (Username, Password) VALUES
('admin', 'e10adc3949ba59abbe56e057f20f883e'); -- 123456 的 MD5
```

---

### 1.3 数据库关系说明

| 关系 | 类型 | 说明 |
|------|------|------|
| Seat ← Reservation | 一对多 | 一个座位可以有多个预约 |
| Reservation → Seat | 多对一 | 一个预约对应一个座位 |

---

### 1.4 数据库初始化脚本

```sql
-- 创建数据库
CREATE DATABASE LibrarySeatReservation;
GO

USE LibrarySeatReservation;
GO

-- 创建 Seat 表
CREATE TABLE Seat (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Location NVARCHAR(100) NOT NULL,
    HasPower BIT NOT NULL DEFAULT 0,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- 创建 Reservation 表
CREATE TABLE Reservation (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SeatId INT NOT NULL,
    UserName NVARCHAR(50) NOT NULL,
    ReserveDate DATE NOT NULL,
    TimeSlot NVARCHAR(20) NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Reservation_Seat FOREIGN KEY (SeatId) REFERENCES Seat(Id),
    CONSTRAINT UQ_Reservation UNIQUE (SeatId, ReserveDate, TimeSlot, Status)
);
GO

-- 创建 AdminUser 表
CREATE TABLE AdminUser (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL
);
GO

-- 创建索引
CREATE INDEX IX_Seat_Status ON Seat(Status);
CREATE INDEX IX_Reservation_SeatId ON Reservation(SeatId);
CREATE INDEX IX_Reservation_UserName ON Reservation(UserName);
CREATE INDEX IX_Reservation_ReserveDate ON Reservation(ReserveDate);
CREATE INDEX IX_Reservation_Status ON Reservation(Status);
GO

-- 插入种子数据
-- ... (参考上面的种子数据)
```

---

## 2. API接口定义

### 2.1 用户端API

---

#### API 1：获取座位列表

**请求**：
```
GET /api/seat/list?page=1&pageSize=10
```

**参数**：
| 参数 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| page | int | 否 | 1 | 页码 |
| pageSize | int | 否 | 10 | 每页条数 |

**响应**：
```json
{
  "success": true,
  "data": {
    "total": 20,
    "page": 1,
    "pageSize": 10,
    "items": [
      {
        "id": 1,
        "name": "A-01",
        "location": "一楼东侧",
        "hasPower": true,
        "status": 0,
        "statusText": "可用"
      }
    ]
  }
}
```

---

#### API 2：获取座位详情

**请求**：
```
GET /api/seat/detail/{id}
```

**参数**：
| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | int | 是 | 座位ID |

**响应**：
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "A-01",
    "location": "一楼东侧",
    "hasPower": true,
    "status": 0,
    "statusText": "可用",
    "timeSlots": [
      {
        "timeSlot": "08:00-10:00",
        "isAvailable": true,
        "reservedBy": null
      },
      {
        "timeSlot": "10:00-12:00",
        "isAvailable": false,
        "reservedBy": "张三"
      }
    ]
  }
}
```

---

#### API 3：提交预约

**请求**：
```
POST /api/reservation/create
```

**请求体**：
```json
{
  "seatId": 1,
  "userName": "张三",
  "reserveDate": "2026-07-14",
  "timeSlot": "14:00-16:00"
}
```

**响应**：
```json
{
  "success": true,
  "message": "预约成功",
  "data": {
    "id": 1,
    "seatName": "A-01",
    "reserveDate": "2026-07-14",
    "timeSlot": "14:00-16:00",
    "status": 0,
    "statusText": "已预约"
  }
}
```

**错误响应**：
```json
{
  "success": false,
  "message": "该时段已被预约，请选择其他时段"
}
```

---

#### API 4：获取我的预约列表

**请求**：
```
GET /api/reservation/my?userName=张三
```

**参数**：
| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| userName | string | 是 | 当前体验账号 |

**响应**：
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "seatId": 1,
      "seatName": "A-01",
      "reserveDate": "2026-07-14",
      "timeSlot": "14:00-16:00",
      "status": 0,
      "statusText": "已预约",
      "createdAt": "2026-07-14 10:30:00"
    }
  ]
}
```

---

#### API 5：取消预约

**请求**：
```
POST /api/reservation/cancel/{id}
```

**参数**：
| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | int | 是 | 预约ID |

**响应**：
```json
{
  "success": true,
  "message": "取消成功"
}
```

**错误响应**：
```json
{
  "success": false,
  "message": "无法取消他人的预约"
}
```

---

#### API 6：切换体验账号

**请求**：
```
POST /api/reservation/switchUser
```

**请求体**：
```json
{
  "userName": "李四"
}
```

**响应**：
```json
{
  "success": true,
  "message": "切换成功",
  "data": {
    "userName": "李四"
  }
}
```

---

### 2.2 管理端API

---

#### API 7：管理员登录

**请求**：
```
POST /api/admin/login
```

**请求体**：
```json
{
  "username": "admin",
  "password": "123456"
}
```

**响应**：
```json
{
  "success": true,
  "message": "登录成功",
  "data": {
    "username": "admin"
  }
}
```

**错误响应**：
```json
{
  "success": false,
  "message": "用户名或密码错误"
}
```

---

#### API 8：管理员登出

**请求**：
```
GET /api/admin/logout
```

**响应**：
```json
{
  "success": true,
  "message": "登出成功"
}
```

---

#### API 9：获取所有座位（管理端）

**请求**：
```
GET /api/admin/seat/list
```

**响应**：
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "A-01",
      "location": "一楼东侧",
      "hasPower": true,
      "status": 0,
      "statusText": "可用",
      "createdAt": "2026-07-14 00:00:00"
    }
  ]
}
```

---

#### API 10：新增座位

**请求**：
```
POST /api/admin/seat/create
```

**请求体**：
```json
{
  "name": "A-06",
  "location": "一楼东侧",
  "hasPower": true,
  "status": 0
}
```

**响应**：
```json
{
  "success": true,
  "message": "新增成功",
  "data": {
    "id": 21,
    "name": "A-06"
  }
}
```

**错误响应**：
```json
{
  "success": false,
  "message": "座位编号已存在"
}
```

---

#### API 11：编辑座位

**请求**：
```
POST /api/admin/seat/edit/{id}
```

**请求体**：
```json
{
  "name": "A-01",
  "location": "一楼东侧（已更新）",
  "hasPower": true,
  "status": 1
}
```

**响应**：
```json
{
  "success": true,
  "message": "编辑成功"
}
```

---

#### API 12：删除座位

**请求**：
```
POST /api/admin/seat/delete/{id}
```

**响应**：
```json
{
  "success": true,
  "message": "删除成功"
}
```

**错误响应**：
```json
{
  "success": false,
  "message": "该座位有预约记录，无法删除"
}
```

---

#### API 13：获取预约列表（管理端）

**请求**：
```
GET /api/admin/reservation/list?date=2026-07-14&status=0
```

**参数**：
| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| date | string | 否 | 按日期筛选 |
| status | int | 否 | 按状态筛选 |

**响应**：
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "seatId": 1,
      "seatName": "A-01",
      "userName": "张三",
      "reserveDate": "2026-07-14",
      "timeSlot": "14:00-16:00",
      "status": 0,
      "statusText": "已预约",
      "createdAt": "2026-07-14 10:30:00"
    }
  ]
}
```

---

#### API 14：获取统计数据

**请求**：
```
GET /api/admin/statistics
```

**响应**：
```json
{
  "success": true,
  "data": {
    "totalReservations": 35,
    "todayReservations": 8,
    "validReservations": 12,
    "topSeats": [
      {
        "seatId": 14,
        "seatName": "C-04",
        "count": 12
      },
      {
        "seatId": 1,
        "seatName": "A-01",
        "count": 8
      }
    ],
    "timeSlotDistribution": [
      {
        "timeSlot": "08:00-10:00",
        "count": 10,
        "percentage": 30.0
      },
      {
        "timeSlot": "10:00-12:00",
        "count": 8,
        "percentage": 25.0
      },
      {
        "timeSlot": "14:00-16:00",
        "count": 13,
        "percentage": 40.0
      },
      {
        "timeSlot": "16:00-18:00",
        "count": 5,
        "percentage": 15.0
      },
      {
        "timeSlot": "19:00-21:00",
        "count": 3,
        "percentage": 10.0
      }
    ]
  }
}
```

---

### 2.3 API接口汇总

| 序号 | 接口 | 方法 | 路径 | 说明 |
|------|------|------|------|------|
| 1 | 获取座位列表 | GET | /api/seat/list | 分页查询 |
| 2 | 获取座位详情 | GET | /api/seat/detail/{id} | 含时段状态 |
| 3 | 提交预约 | POST | /api/reservation/create | 含冲突校验 |
| 4 | 获取我的预约 | GET | /api/reservation/my | 按用户筛选 |
| 5 | 取消预约 | POST | /api/reservation/cancel/{id} | 状态变更 |
| 6 | 切换体验账号 | POST | /api/reservation/switchUser | 更新Session |
| 7 | 管理员登录 | POST | /api/admin/login | Session认证 |
| 8 | 管理员登出 | GET | /api/admin/logout | 清除Session |
| 9 | 获取所有座位 | GET | /api/admin/seat/list | 管理端专用 |
| 10 | 新增座位 | POST | /api/admin/seat/create | 含唯一校验 |
| 11 | 编辑座位 | POST | /api/admin/seat/edit/{id} | 更新信息 |
| 12 | 删除座位 | POST | /api/admin/seat/delete/{id} | 含关联校验 |
| 13 | 获取预约列表 | GET | /api/admin/reservation/list | 支持筛选 |
| 14 | 获取统计数据 | GET | /api/admin/statistics | 实时计算 |

---

## 3. 数据验证规则

### 3.1 Seat 表验证

| 字段 | 规则 | 错误信息 |
|------|------|----------|
| Name | 不能为空 | "座位编号不能为空" |
| Name | 最大长度 50 | "座位编号不能超过50个字符" |
| Name | 不可重复 | "座位编号已存在" |
| Location | 不能为空 | "位置不能为空" |
| Location | 最大长度 100 | "位置不能超过100个字符" |

### 3.2 Reservation 表验证

| 字段 | 规则 | 错误信息 |
|------|------|----------|
| SeatId | 必须存在 | "座位不存在" |
| UserName | 不能为空 | "预约人不能为空" |
| ReserveDate | 不能为空 | "预约日期不能为空" |
| ReserveDate | 必须 >= 今天 | "不能预约过去的日期" |
| TimeSlot | 必须是枚举值之一 | "时段无效" |
| 冲突校验 | 同座位同时段同时Status=0不可重复 | "该时段已被预约" |

### 3.3 AdminUser 表验证

| 字段 | 规则 | 错误信息 |
|------|------|----------|
| Username | 不能为空 | "用户名不能为空" |
| Password | 不能为空 | "密码不能为空" |
| Password | MD5匹配 | "用户名或密码错误" |

---

## 4. 业务规则

### 4.1 预约规则

| 规则 | 说明 |
|------|------|
| 日期范围 | 只能预约今天及之后的日期 |
| 时段限制 | 只能选择固定5个时段 |
| 冲突检测 | 同一座位+同一日期+同一时段+Status=0不可重复 |
| 用户限制 | 同一用户同一时段只能预约一个座位（不同座位可以） |

### 4.2 取消规则

| 规则 | 说明 |
|------|------|
| 权限 | 只能取消自己的预约 |
| 状态限制 | 只能取消 Status=0 的预约 |
| 操作方式 | 仅支持 POST 方式 |
| 结果 | Status 从 0 变为 2 |

### 4.3 座位管理规则

| 规则 | 说明 |
|------|------|
| 编号唯一 | 新增/编辑时校验编号唯一性 |
| 删除限制 | 有预约记录（Status != 2）的座位不能删除 |
| 状态变更 | 可在"可用"和"维护中"之间切换 |

### 4.4 统计规则

| 规则 | 说明 |
|------|------|
| 总预约数 | 所有记录，包含所有状态 |
| 今日预约数 | ReserveDate = 今天 且 Status != 2 |
| 有效预约数 | Status = 0 |
| 热门座位 | 排除 Status=2，按 SeatId 分组 COUNT 降序 |
| 时段分布 | 排除 Status=2，按 TimeSlot 分组 COUNT |

---

**本文档完成后，下一步输出：开发任务分解。**