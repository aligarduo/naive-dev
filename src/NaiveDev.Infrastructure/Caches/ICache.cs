using NaiveDev.Infrastructure.Enums;

namespace NaiveDev.Infrastructure.Caches
{
    /// <summary>
    /// 缓存服务接口
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        void SetCache(string key, object value);

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        Task SetCacheAsync(string key, object value, CancellationToken token = default);

        /// <summary>
        /// 设置缓存项默认过期类型为绝对过期
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeout">过期时间间隔</param>
        void SetCache(string key, object value, TimeSpan timeout);

        /// <summary>
        /// 设置缓存项默认过期类型为绝对过期
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeout">过期时间间隔</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        Task SetCacheAsync(string key, object value, TimeSpan timeout, CancellationToken token = default);

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeout">过期时间间隔</param>
        /// <param name="expireType">过期类型</param>  
        void SetCache(string key, object value, TimeSpan timeout, ExpireType expireType);

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeout">过期时间间隔</param>
        /// <param name="expireType">过期类型</param>  
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        Task SetCacheAsync(string key, object value, TimeSpan timeout, ExpireType expireType, CancellationToken token = default);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        string GetCache(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        Task<string> GetCacheAsync(string key, CancellationToken token = default);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        T? GetCache<T>(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        Task<T?> GetCacheAsync<T>(string key, CancellationToken token = default);

        /// <summary>
        /// 是否存在缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        bool IsAny(string key);

        /// <summary>
        /// 是否存在缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        Task<bool> IsAnyAsync(string key, CancellationToken token = default);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        void RemoveCache(string key);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        Task RemoveCacheAsync(string key, CancellationToken token = default);

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        void RefreshCache(string key);

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        Task RefreshCacheAsync(string key, CancellationToken token = default);

        /// <summary>
        /// 根据指定的缓存键获取缓存的值，如果缓存不存在则使用提供的函数计算结果并设置到缓存中
        /// </summary>
        /// <typeparam name="T">缓存值的类型</typeparam>
        /// <param name="key">缓存键，用于标识缓存中的项</param>
        /// <param name="func">用于计算缓存值的函数，当缓存项不存在时调用</param>
        /// <param name="timeout">缓存项的过期时间间隔</param>
        /// <param name="token">取消令牌，用于取消正在进行的异步操作</param>
        /// <returns>返回一个表示异步操作的任务，该任务在完成时包含缓存项的值或计算得到的值</returns>
        Task<T> GetOrSet<T>(string key, Func<T> func, TimeSpan timeout, CancellationToken token = default);
    }
}
