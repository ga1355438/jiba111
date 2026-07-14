# 静态原型文件说明

## 文件列表

| 序号 | 文件名 | 页面名称 | 原型精度 |
|------|--------|----------|----------|
| 1 | 01-user-home.html | 用户首页 | 中保真 |
| 2 | 02-seat-list.html | 座位列表页 | 中保真 |
| 3 | 03-seat-detail.html | 座位详情页 | 中保真 |
| 4 | 04-reservation-create.html | 预约提交页 | 中保真 |
| 5 | 05-my-reservations.html | 我的预约页 | 中保真 |
| 6 | 06-admin-login.html | 管理员登录页 | 中保真 |
| 7 | 07-admin-seat.html | 座位管理页 | 中保真 |
| 8 | 08-admin-reservation.html | 预约管理页 | 中保真 |
| 9 | 09-admin-statistics.html | 统计页 | 中保真 |

## 使用方式

直接在浏览器中打开 HTML 文件即可查看原型效果。

## 响应式说明

- **桌面端**：表格布局，宽度 > 640px
- **手机端**：卡片布局，宽度 ≤ 640px

可通过浏览器开发者工具切换设备视图查看响应式效果。

## 文件依赖

所有页面共享 `shared.css` 样式文件，请确保目录结构完整：
```
prototypes/
├── shared.css
├── 01-user-home.html
├── 02-seat-list.html
├── 03-seat-detail.html
├── 04-reservation-create.html
├── 05-my-reservations.html
├── 06-admin-login.html
├── 07-admin-seat.html
├── 08-admin-reservation.html
└── 09-admin-statistics.html
```

## 原型说明

1. 本原型为**静态 HTML 原型**，不包含后端逻辑
2. 弹窗、下拉菜单等交互效果可通过点击触发（部分已预置展示状态）
3. 实际开发时需根据后端接口调整数据展示
