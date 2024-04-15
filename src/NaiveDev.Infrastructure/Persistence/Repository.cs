using SqlSugar;

namespace NaiveDev.Infrastructure.Persistence
{
    /// <summary>
    /// 泛型仓储类，用于对指定类型T的实体进行数据库操作。
    /// 该类依赖于IUnitOfWork接口实现单位工作流，通过单位工作流获取数据库操作客户端SqlSugarClient。
    /// 支持多租户模式，根据实体类上的TenantAttribute特性切换数据库。
    /// </summary>  
    /// <typeparam name="T">需要进行数据库操作的实体类型，要求该类型是可创建且是类。</typeparam>  
    public class Repository<T>(IUnitOfWork unitOfWork) : IRepository<T>
    {
        /// <summary>
        /// 数据库操作基础客户端实例，由传入的IUnitOfWork对象初始化并赋值
        /// </summary>
        private readonly SqlSugarClient dbBase = unitOfWork.GetDbClient();

        /// <summary>
        /// 获取数据库操作客户端实例，根据实体类型T是否带有TenantAttribute特性来切换数据库
        /// </summary>
        private ISqlSugarClient DB
        {
            get
            {
                // 检查实体类型T是否带有TenantAttribute特性
                if (typeof(T).GetTypeInfo().GetCustomAttributes(typeof(TenantAttribute), true).FirstOrDefault(q => q.GetType() == typeof(TenantAttribute)) is TenantAttribute Tenant)
                {
                    // 如果带有TenantAttribute特性，则根据特性中的configId切换数据库
                    dbBase.ChangeDatabase(Tenant.configId);
                }
                else
                {
                    // 如果不带有TenantAttribute特性，则切换到默认数据库
                    dbBase.ChangeDatabase(0);
                }

                // 返回数据库操作客户端实例
                return dbBase;
            }
        }

        /// <summary>
        /// 返回一个可查询的接口，用于构建和执行针对实体<typeparamref name="T"/>的查询
        /// </summary>
        /// <returns>返回一个<see cref="ISugarQueryable{T}"/>的查询对象，允许对<typeparamref name="T"/>的实体进行查询</returns>
        public ISugarQueryable<T> Queryable()
        {
            return DB.Queryable<T>();
        }

        /// <summary>
        /// 返回一个可插入的对象
        /// </summary>
        /// <param name="t">要插入的实体对象，类型为<typeparamref name="T"/></param>
        /// <returns>返回一个<see cref="IInsertable{T}"/>的插入对象，该对象可用于执行对类型<typeparamref name="T"/>的实体的插入操作</returns>
        public InsertMethodInfo Insertable(T t)
        {
            return DB.InsertableByObject(t);
        }
    }
}
