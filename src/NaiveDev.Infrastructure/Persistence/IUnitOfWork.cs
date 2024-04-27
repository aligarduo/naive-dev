using System.Data;
using NaiveDev.Infrastructure.Service;
using SqlSugar;

namespace NaiveDev.Infrastructure.Persistence
{
    /// <summary>
    /// 定义单位工作流接口，用于管理数据库事务的开启、提交、回滚，以及提供数据库客户端实例。  
    /// 该接口继承了ITransitDenpendency接口，可能用于依赖注入或事务管理。  
    /// </summary>
    public interface IUnitOfWork : ITransientDependency
    {
        /// <summary>
        /// 获取用于数据库操作的SqlSugarClient实例
        /// </summary>
        /// <returns>返回SqlSugarClient实例</returns>
        SqlSugarClient GetDbClient();

        /// <summary>
        /// 开始一个新的事务
        /// </summary>
        void BeginTran();

        /// <summary>
        /// 使用指定的事务隔离级别开始一个新的事务
        /// </summary>
        /// <param name="isolationLevel">事务的隔离级别</param>
        void BeginTran(IsolationLevel isolationLevel);

        /// <summary>
        /// 提交当前事务
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 回滚当前事务
        /// </summary>
        void RollbackTran();
    }
}