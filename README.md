<p align="center">
  <img width="144px" src="http://oss.api.huhu.chat/tos-cn-hcd5roe8ym/ea3c5d38-a564-4ecf-a8df-92425d4261dc.svg" />
</p>
<h1 align="center">Naive Dev</h1>
<p align="center">一个 .NET WebAPI 开发框架</p>
<p align="center"><b>领域驱动，开箱即用</b></p>
<p align="center">只需要定义 Entitie 与 DTO，就可以快速完成 Restful 风格的 WebAPI 接口开发</p>



## 支持.NET
.net8 .net9


## 支持数据库

MySQL、SqlServer、Sqlite、Oracle 、 PostgreSQL

## 特性特点

- [x] 基于令牌桶实现IP限流
- [x] IP黑白名单
- [ ] 接口幂等性
- [x] 接口事务回滚
- [x] 多库读写
- [x] CQRS

## 目录结构

├─NaiveDev.WebHost
│ └─Controllers
├─NaiveDev.Application 
│ ├─CommandHandlers 
│ ├─Commands
│ ├─Dtos
│ ├─QuerieHandlers
│ └─Queries
├─NaiveDev.Domain 
│ ├─Entities 
│ └─ValueObjects 
├─NaiveDev.Infrastructure 
│ ├─Attributes 
│ ├─Auth 
│ ├─Caches 
│ ├─Commons 
│ ├─Data 
│ ├─Entities 
│ ├─Enums 
│ ├─Extensions 
│ ├─Internet 
│ ├─Jwt 
│ ├─Middleware 
│ ├─Persistence 
│ ├─Service 
│ └─Tools 
└─NaiveDev.Shared 

## 许可

NaiveDev 使用 [MIT license](https://opensource.org/licenses/MIT) 许可证书。