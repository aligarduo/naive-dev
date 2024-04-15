using NaiveDev.Infrastructure.Enums;

namespace NaiveDev.Infrastructure.Data
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public class CacheConfiguration
    {
        /// <summary>
        /// 缓存配置
        /// </summary>
        public CacheConfiguration()
        {
            CacheType = CacheType.Memory;
            RedisEndpoint = string.Empty;
        }

        /// <summary>
        /// Memory OR Redis
        /// </summary>
        public CacheType CacheType { get; set; }

        /// <summary>
        /// Redis节点地址
        /// </summary>
        public string RedisEndpoint { get; set; }
    }
}
