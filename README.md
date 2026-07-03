# DDD 六层架构用户管理系统

基于.NET 8 + Vue 3 实现的六层领域驱动设计（DDD）架构用户登录和用户管理系统。

## 技术栈

### 后端
- **框架**: ASP.NET Core Web API 8.0
- **ORM**: Entity Framework Core 8.0 (MySQL)
- **认证**: JWT + Refresh Token
- **CQRS**: MediatR
- **对象映射**: AutoMapper
- **日志**: Serilog（控制台+文件）
- **依赖注入**: 特性自动注册

### 前端
- **框架**: Vue 3 + TypeScript
- **状态管理**: Pinia
- **HTTP客户端**: Axios 封装（utils/request）
- **构建工具**: Vite

## 架构分层

1. **DDD.Presentation** - 表现层（API控制器、中间件）
2. **DDD.Application** - 应用层（命令、查询、DTO）
3. **DDD.Domain** - 领域层（聚合根、实体、值对象、领域事件、领域服务）
4. **DDD.Infrastructure** - 基础设施层（数据访问、第三方服务实现）
5. **DDD.Shared** - 共享层（通用工具、过滤器、中间件、特性）

## 核心特性

- ✅ 领域驱动设计（DDD）六层架构
- ✅ 聚合根 + 领域事件 + 领域服务
- ✅ 在 SaveChanges 中自动发布领域事件
- ✅ CQRS 模式通过 MediatR 实现
- ✅ JWT + Refresh Token 双令牌认证
- ✅ 依赖注入使用特性自动注册
- ✅ 全局异常处理中间件
- ✅ Action 过滤器完整日志记录（支持敏感信息脱敏）
- ✅ Serilog 日志同时输出到控制台和本地文件
- ✅ 通用泛型仓储模式
- ✅ MySQL 数据库 + 种子数据
- ✅ 现代化白色主题前端UI

## 项目结构

```
├── backend/
│   ├── src/
│   │   ├── DDD.Presentation/      # 表现层
│   │   ├── DDD.Application/       # 应用层
│   │   ├── DDD.Domain/            # 领域层
│   │   ├── DDD.Infrastructure/    # 基础设施层
│   │   └── DDD.Shared/            # 共享层
│   └── DDD.sln
└── frontend/
    ├── src/
    │   ├── views/                 # 页面组件
    │   ├── stores/                # Pinia 状态管理
    │   ├── utils/                 # 工具（request.ts）
    │   └── ...
    └── package.json
```

## 快速开始

### 后端启动

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project src/DDD.Presentation/DDD.Presentation.csproj --urls http://localhost:5000
```

### 前端启动

```bash
cd frontend
npm install
npm run dev
```

访问 http://localhost:5173

## 默认账户

| 用户名 | 邮箱 | 密码 | 角色 |
|--------|------|------|------|
| 管理员 | admin@example.com | Admin123! | Admin |
| 普通用户 | user@example.com | User123! | User |

## 日志功能

- 请求开始/结束日志记录
- 显示请求方法、完整URL、客户端IP、控制器、Action
- 自动脱敏敏感字段（password、token、secret等）
- 同时输出到控制台和 `Logs/` 目录下按天分卷的文件
- 保留最近 30 天日志

## API 端点

- `POST /api/auth/login` - 用户登录
- `POST /api/auth/register` - 用户注册
- `GET /api/users` - 获取用户列表（需要Admin权限）
- `DELETE /api/users/{id}` - 删除用户（需要Admin权限）

## 许可证

MIT