using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using NaiveDev.Infrastructure.Commons;

using SqlSugar;

namespace NaiveDev.Infrastructure.Middleware
{
    /// <summary>
    /// 事务中间件类，用于在ASP.NET Core请求管道中管理数据库事务
    /// </summary>
    public static class TransactionMiddleWare
    {
        /// <summary>
        /// 在.NET应用程序的请求管道中添加中间件，用于使用数据库事务包裹整个请求处理过程
        /// </summary>
        /// <param name="app">IApplicationBuilder对象，表示.NET应用程序的请求管道构建器</param>  
        public static void UseTransaction(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                // 从请求服务的容器中获取数据库上下文对象ISqlSugarClient的实例
                var db = context.RequestServices.GetRequiredService<ISqlSugarClient>();

                try
                {
                    // 开始一个数据库事务
                    await db.Ado.BeginTranAsync();

                    // 执行下一个中间件
                    await next();

                    // 如果所有操作都成功，则提交事务
                    await db.Ado.CommitTranAsync();
                }
                catch (Exception ex)
                {
                    // 如果在请求处理过程中发生异常，则回滚事务 
                    await db.Ado.RollbackTranAsync();

                    await ContextResponse.ImmediateReturn(context, 500, ex.Message);
                }
            });
        }
    }
}