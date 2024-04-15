using NaiveDev.Infrastructure.Service;

using SqlSugar;

namespace NaiveDev.Infrastructure.Persistence
{
    /// <summary>
    /// 单位工作流实现类，用于管理数据库事务并提供数据库操作客户端。  
    /// 继承自ServiceBase类，实现IUnitOfWork接口。  
    /// </summary>
    /// <param name="sqlSugarClient">SqlSugar数据库操作客户端实例</param>
    public class UnitOfWork(ISqlSugarClient sqlSugarClient) : ServiceBase, IUnitOfWork
    {
        /// <summary>
        /// SqlSugar数据库操作客户端实例，用于执行数据库操作
        /// </summary>
        private readonly ISqlSugarClient _sqlSugarClient = sqlSugarClient;

        /// <summary>
        /// 获取SqlSugarClient实例，用于数据库操作
        /// </summary>
        /// <returns>返回SqlSugarClient实例</returns>
        /// <exception cref="Exception">当_sqlSugarClient无法转换为SqlSugarClient类型时，抛出异常</exception>
        public SqlSugarClient GetDbClient()
        {
            SqlSugarClient? sqlSugarClient = _sqlSugarClient as SqlSugarClient;
            return sqlSugarClient ?? throw new Exception("The database client cannot be null");
        }

        /// <summary>
        /// 开启一个新的事务
        /// </summary>
        public void BeginTran()
        {
            GetDbClient().Ado.BeginTran();
        }

        /// <summary>
        /// 使用指定的事务隔离级别开启一个新的事务
        /// </summary>
        /// <param name="isolationLevel">事务的隔离级别</param>
        public void BeginTran(System.Data.IsolationLevel isolationLevel)
        {
            GetDbClient().Ado.BeginTran(isolationLevel);
        }

        /// <summary>
        /// 提交当前事务
        /// 如果提交过程中发生异常，则回滚事务并抛出异常
        /// </summary>
        public void CommitTran()
        {
            try
            {
                GetDbClient().Ado.CommitTran();
            }
            catch
            {
                GetDbClient().Ado.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 回滚当前事务
        /// </summary> 
        public void RollbackTran()
        {
            GetDbClient().Ado.RollbackTran();
        }
    }
}
