using Microsoft.Extensions.DependencyInjection;

using NaiveDev.Infrastructure.Persistence;

namespace NaiveDev.Infrastructure.Extensions
{
    /// <summary>
    /// 仓储相关的扩展方法类，提供IServiceCollection的扩展方法来注册仓储接口及其实现
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// 为IServiceCollection添加仓储接口及其实现的扩展方法
        /// </summary>
        /// <param name="Services">IServiceCollection实例，用于配置应用程序的服务</param>
        /// <returns>配置后的IServiceCollection实例</returns>
        public static IServiceCollection AddRepository(this IServiceCollection Services)
        {
            // 注册仓储接口IRepository<>及其泛型实现Repository<>为Transient生命周期的服务  
            // 这意味着每次请求仓储服务时，都会创建一个新的Repository实例  
            Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            // 返回配置后的IServiceCollection实例，以便链式调用其他配置方法  
            return Services;
        }
    }
}
