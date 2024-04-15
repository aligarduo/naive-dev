using SqlSugar;

namespace NaiveDev.Infrastructure.Persistence
{
    /// <summary>
    /// 泛型仓储接口，用于操作特定类型的实体对象
    /// </summary>
    /// <typeparam name="T">实体对象的类型，需要是一个可创建实例的类</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// 返回一个可查询的接口，用于构建和执行针对实体<typeparamref name="T"/>的查询
        /// </summary>
        /// <returns>返回一个<see cref="ISugarQueryable{T}"/>的查询对象，允许对<typeparamref name="T"/>的实体进行查询</returns>
        ISugarQueryable<T> Queryable();

        /// <summary>
        /// 返回一个可插入的对象
        /// </summary>
        /// <param name="t">要插入的实体对象，类型为<typeparamref name="T"/></param>
        /// <returns>返回一个<see cref="IInsertable{T}"/>的插入对象，该对象可用于执行对类型<typeparamref name="T"/>的实体的插入操作</returns>
        InsertMethodInfo Insertable(T t);
    }
}
