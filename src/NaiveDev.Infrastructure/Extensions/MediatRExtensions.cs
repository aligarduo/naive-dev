using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace NaiveDev.Infrastructure.Extensions
{
    /// <summary>
    /// MediatR扩展类，用于扩展IServiceCollection的功能，以便能够方便地添加MediatR及其依赖注入支持
    /// </summary>
    public static class MediatRExtensions
    {
        /// <summary>
        /// 向IServiceCollection中添加MediatR及其依赖注入支持的扩展方法
        /// </summary>
        /// <param name="services">IServiceCollection实例，用于注册服务和依赖注入</param>
        /// <param name="executingAssembly">当前正在执行的程序集</param>
        public static void AddMediatR(this IServiceCollection services, Assembly executingAssembly)
        {
            // 获取当前正在执行的程序集所依赖的所有程序集
            var referencedAssemblies = executingAssembly.GetReferencedAssemblies();

            // 创建一个包含当前执行程序集及其所有依赖程序集的集合
            var assemblies = new Assembly[] { executingAssembly }
                .Concat(referencedAssemblies.Select(Assembly.Load)) // 加载所有依赖程序集
                .ToArray(); // 将集合转换为数组

            // 过滤出所有包含非抽象、非接口的IBaseRequest继承类的程序集
            var finalAssemblies = assemblies
                .Where(a => a.GetTypes().Any(t => typeof(IBaseRequest).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface))
                .Distinct() // 去除重复的程序集
                .ToArray(); // 将过滤后的集合转换为数组

            // 如果没有找到任何含有IBaseRequest子类型的程序集，则直接返回，不执行后续操作
            if (finalAssemblies.Length == 0) return;

            // 注册MediatR及其依赖注入支持
            // 这里使用了一个lambda表达式来配置MediatR，使其能够扫描finalAssemblies中的程序集，并自动注册相应的请求处理器和服务
            services.AddMediatR((config) =>
            {
                config.RegisterServicesFromAssemblies(finalAssemblies);
            });
        }
    }
}
