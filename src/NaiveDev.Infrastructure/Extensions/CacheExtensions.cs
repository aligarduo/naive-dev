using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NaiveDev.Infrastructure.Caches;
using NaiveDev.Infrastructure.Data;
using NaiveDev.Infrastructure.Enums;

namespace NaiveDev.Infrastructure.Extensions
{
    /// <summary>
    /// Cache相关的扩展方法类，提供IHostBuilder的扩展方法来集成Cache作为依赖注入容器
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// 为 IHostBuilder 添加缓存配置扩展方法
        /// </summary>
        /// <param name="hostBuilder">IHostBuilder 实例，用于配置主机</param>
        /// <returns>配置过缓存的 IHostBuilder 实例</returns>
        public static IHostBuilder AddCache(this IHostBuilder hostBuilder)
        {
            // 配置服务容器，添加缓存相关服务
            hostBuilder.ConfigureServices((buidlerContext, services) =>
            {
                // 将自定义的 ICache 接口实现 Cache 注册为单例服务
                services.AddSingleton<ICache, Cache>();

                // 从配置文件中获取 Cache 配置节的信息，并映射为 CacheConfiguration 类型对象
                CacheConfiguration? cacheOption = buidlerContext.Configuration.GetSection("Cache").Get<CacheConfiguration>();

                // 根据 CacheConfiguration 中的 CacheType 属性值，决定使用哪种缓存类型
                switch (cacheOption?.CacheType)
                {
                    // 使用内存缓存
                    case CacheType.Memory:
                        // 添加内存缓存服务到服务容器中
                        services.AddDistributedMemoryCache();
                        break;

                    // 使用 Redis 缓存
                    case CacheType.Redis:
                        {
                            // 创建 CSRedisClient 实例，用于连接 Redis 服务器
                            var csredis = new CSRedisClient(cacheOption.RedisEndpoint);

                            // 初始化 RedisHelper，用于简化 Redis 操作
                            RedisHelper.Initialization(csredis);

                            // 将 CSRedisClient 实例注册为单例服务，方便在应用程序中使用
                            services.AddSingleton(csredis);

                            // 使用 CSRedisCache 实现 IDistributedCache 接口，并注册为单例服务
                            // Caching.CSRedis 是基于 CSRedisClient 的 Redis 缓存封装
                            services.AddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));
                        };
                        break;

                    // 如果 CacheType 无效，则抛出异常
                    default: throw new Exception("Invalid cache type");
                }
            });

            // 返回配置过缓存的 IHostBuilder 实例
            return hostBuilder;
        }
    }
}
