using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NaiveDev.Infrastructure.Service;

namespace NaiveDev.Infrastructure.Extensions
{
    /// <summary>
    /// Autofac相关的扩展方法类，提供IHostBuilder的扩展方法来集成Autofac作为依赖注入容器
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// 为IHostBuilder添加Autofac作为依赖注入容器的扩展方法
        /// </summary>
        /// <param name="Host">IHostBuilder实例，用于配置应用程序的主机</param>
        /// <param name="executingAssembly">当前正在执行的程序集</param>
        /// <returns>配置后的IHostBuilder实例</returns>
        public static IHostBuilder AddAutofac(this IHostBuilder Host, Assembly executingAssembly)
        {
            // 设置Autofac为服务提供程序工厂
            Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            // 配置Autofac容器，注册AutofacRegisterModule模块
            Host.ConfigureContainer<ContainerBuilder>((hostBuilderContext, containerBuilder) =>
            {
                // 注册AutofacRegisterModule，并传递当前执行的程序集
                containerBuilder.RegisterModule(new AutofacRegisterModule(executingAssembly));
            });

            // 返回配置后的IHostBuilder实例，以便链式调用其他配置方法
            return Host;
        }
    }

    /// <summary>
    /// Autofac模块注册类，用于在Autofac容器中注册程序集中的服务
    /// </summary>
    /// <param name="executingAssembly">当前正在执行的程序集</param>
    public class AutofacRegisterModule(Assembly executingAssembly) : Autofac.Module
    {
        /// <summary>
        /// 当前正在执行的程序集
        /// </summary>
        private readonly Assembly _executingAssembly = executingAssembly;

        /// <summary>
        /// 重写Load方法，用于加载和注册服务
        /// </summary>
        /// <param name="builder">Autofac的容器构建器对象</param>
        protected override void Load(ContainerBuilder builder)
        {
            // 获取当前执行程序集所依赖的所有程序集  
            var referencedAssemblies = _executingAssembly.GetReferencedAssemblies();

            // 获取所有程序集的集合（包括当前执行程序集和它的依赖）  
            var assemblies = new Assembly[] { _executingAssembly }
                .Concat(referencedAssemblies.Select(Assembly.Load))
                .ToArray();

            // 扫描所有程序集，找到所有继承自ServiceBase的类型
            var finalAssemblies = assemblies
                .Where(a => a.GetTypes().Any(t => t.IsSubclassOf(typeof(ServiceBase))))
                .Distinct()
                .ToArray();

            // 如果没有找到任何含有ServiceBase子类型的程序集，则直接返回，不执行后续操作
            if (finalAssemblies.Length == 0) return;

            // 遍历所有含有ServiceBase子类型的程序集  
            foreach (var assembly in finalAssemblies)
            {
                // 使用Autofac的扩展方法BatchAutowired，批量自动装配（注册）该程序集中的组件
                builder.BatchAutowired(assembly);
            }
        }
    }

    /// <summary>
    /// 依赖注入管理器，用于管理和配置依赖注入的容器
    /// </summary>
    public static class IocManager
    {
        /// <summary>
        /// 批量自动注入扩展方法，用于批量注册指定程序集中的类型到依赖注入容器中
        /// </summary>
        /// <param name="builder">Autofac容器构建器对象</param>
        /// <param name="assembly">需要注入的类型所在的程序集</param>
        public static void BatchAutowired(this ContainerBuilder builder, Assembly assembly)
        {
            // 定义瞬时生命周期的接口类型。瞬时注入，每次请求都创建一个新的实例
            var transientType = typeof(ITransientDependency);

            // 定义单例生命周期的接口类型。单例注入，在整个应用程序生命周期中只创建一个实例
            var singletonType = typeof(ISingletonDependency);

            // 定义作用域生命周期的接口类型。作用域注入，每次请求作用域开始时创建一个新的实例，作用域结束时销毁
            var scopeType = typeof(IScopeDependency);

            // 批量注册瞬时生命周期的类型
            // 在指定程序集中查找所有满足条件的类型（是类、非抽象、实现了ITransitDenpendency接口），
            // 并以自身类型和实现的接口类型注册到容器中，生命周期为每次请求时创建一个新实例。
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(transientType))
                .AsSelf() // 注册类型自身  
                .AsImplementedInterfaces() // 注册类型实现的所有接口
                .InstancePerDependency(); // 每次请求时创建一个新实例

            // 批量注册单例生命周期的类型
            // 在指定程序集中查找所有满足条件的类型（是类、非抽象、实现了ISingletonDenpendency接口），
            // 并以自身类型和实现的接口类型注册到容器中，生命周期为单例。
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(singletonType))
                .AsSelf() // 注册类型自身
                .AsImplementedInterfaces() // 注册类型实现的所有接口
                .SingleInstance(); // 单例模式，整个应用程序生命周期中只创建一个实例

            // 批量注册作用域生命周期的类型
            // 在指定程序集中查找所有满足条件的类型（是类、非抽象、实现了IScopeDenpendency接口），
            // 并以自身类型和实现的接口类型注册到容器中，生命周期为请求作用域。
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(scopeType))
                .AsSelf() // 注册类型自身
                .AsImplementedInterfaces() // 注册类型实现的所有接口
                .InstancePerLifetimeScope(); // 每次请求作用域开始时创建一个新实例，作用域结束时销毁
        }
    }
}
