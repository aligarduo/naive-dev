using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using NaiveDev.Infrastructure.Enums;
using NaiveDev.Infrastructure.Tools;

namespace NaiveDev.Infrastructure.Caches
{
    /// <summary>
    /// 缓存服务实现
    /// </summary>
    public class Cache(IDistributedCache cache) : ICache
    {
        private readonly IDistributedCache _cache = cache;

        /// <summary>
        /// 构建缓存Key
        /// </summary>
        /// <param name="idKey"></param>
        /// <returns></returns>
        protected static string BuildKey(string idKey)
        {
            return $"{idKey}";
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        public void SetCache(string key, object value)
        {
            string cacheKey = BuildKey(key);
            _cache.SetString(cacheKey, value.ToJson());
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        public async Task SetCacheAsync(string key, object value, CancellationToken token = default)
        {
            string cacheKey = BuildKey(key);
            await _cache.SetStringAsync(cacheKey, value.ToJson(), token);
        }

        /// <summary>
        /// 设置缓存项默认过期类型为绝对过期
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeout">过期时间间隔</param>
        public void SetCache(string key, object value, TimeSpan timeout)
        {
            string cacheKey = BuildKey(key);
            _cache.SetString(cacheKey, value.ToJson(), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now + timeout)
            });
        }

        /// <summary>
        /// 设置缓存项默认过期类型为绝对过期
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeout">过期时间间隔</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        public async Task SetCacheAsync(string key, object value, TimeSpan timeout, CancellationToken token = default)
        {
            string cacheKey = BuildKey(key);
            await _cache.SetStringAsync(cacheKey, value.ToJson(), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now + timeout)
            }, token);
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeout">过期时间间隔</param>
        /// <param name="expireType">过期类型</param>  
        public void SetCache(string key, object value, TimeSpan timeout, ExpireType expireType)
        {
            string cacheKey = BuildKey(key);
            if (expireType == ExpireType.Absolute)
            {
                _cache.SetString(cacheKey, value.ToJson(), new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now + timeout)
                });
            }
            else
            {
                _cache.SetString(cacheKey, value.ToJson(), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timeout
                });
            }
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeout">过期时间间隔</param>
        /// <param name="expireType">过期类型</param>  
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        public async Task SetCacheAsync(string key, object value, TimeSpan timeout, ExpireType expireType, CancellationToken token = default)
        {
            string cacheKey = BuildKey(key);
            if (expireType == ExpireType.Absolute)
            {
                await _cache.SetStringAsync(cacheKey, value.ToJson(), new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now + timeout)
                }, token);
            }
            else
            {
                await _cache.SetStringAsync(cacheKey, value.ToJson(), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timeout
                }, token);
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public string GetCache(string key)
        {
            if (key == null)
                return string.Empty;

            string cacheKey = BuildKey(key);
            return _cache.GetString(cacheKey) ?? string.Empty;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        public async Task<string> GetCacheAsync(string key, CancellationToken token = default)
        {
            if (key == null)
                return string.Empty;

            string cacheKey = BuildKey(key);
            return await _cache.GetStringAsync(cacheKey, token) ?? string.Empty;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public T? GetCache<T>(string key)
        {
            var cache = GetCache(key);
            if (cache != null)
            {
                return cache.To<T>();
            }
            return default;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        public async Task<T?> GetCacheAsync<T>(string key, CancellationToken token = default)
        {
            var cache = await GetCacheAsync(key, token);
            if (!string.IsNullOrEmpty(cache))
            {
                return cache.To<T>();
            }
            return default;
        }

        /// <summary>
        /// 是否存在缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public bool IsAny(string key)
        {
            var cache = GetCache(key);
            if (!string.IsNullOrEmpty(cache))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否存在缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        public async Task<bool> IsAnyAsync(string key, CancellationToken token = default)
        {
            var cache = await GetCacheAsync(key, token);
            if (!string.IsNullOrEmpty(cache))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public void RemoveCache(string key)
        {
            _cache.Remove(BuildKey(key));
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        public async Task RemoveCacheAsync(string key, CancellationToken token = default)
        {
            await _cache.RemoveAsync(BuildKey(key), token);
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public void RefreshCache(string key)
        {
            _cache.Refresh(BuildKey(key));
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token">取消令牌，用于取消异步操作</param>
        /// <returns>一个表示异步操作的任务</returns>
        public async Task RefreshCacheAsync(string key, CancellationToken token = default)
        {
            await _cache.RefreshAsync(BuildKey(key), token);
        }

        /// <summary>
        /// 根据指定的缓存键获取缓存的值，如果缓存不存在则使用提供的函数计算结果并设置到缓存中
        /// </summary>
        /// <typeparam name="T">缓存值的类型</typeparam>
        /// <param name="key">缓存键，用于标识缓存中的项</param>
        /// <param name="func">用于计算缓存值的函数，当缓存项不存在时调用</param>
        /// <param name="timeout">缓存项的过期时间间隔</param>
        /// <param name="token">取消令牌，用于取消正在进行的异步操作</param>
        /// <returns>返回一个表示异步操作的任务，该任务在完成时包含缓存项的值或计算得到的值</returns>
        public async Task<T> GetOrSet<T>(string key, Func<T> func, TimeSpan timeout, CancellationToken token = default)
        {
            var cache = await GetCacheAsync<T>(key, token);
            if (cache is not null)
            {
                return cache;
            }

            var result = func() ?? throw new InvalidOperationException();
            await SetCacheAsync(key, result, timeout, token);

            return result;
        }
    }
}
