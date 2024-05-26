using Microsoft.AspNetCore.Http;
using NaiveDev.Infrastructure.Caches;
using NaiveDev.Infrastructure.Commons;

namespace NaiveDev.Infrastructure.Middleware
{
    /// <summary>
    /// 限制请求速率中间件
    /// </summary>
    /// <remarks>
    /// API请求限制：
    /// -为了保护服务器资源，我们的API对每个用户都设置了请求频率限制
    /// -如果您的请求频率超过了限制，API将返回状态码429（Too Many Requests）
    /// -当遇到这个错误时，请降低您的请求频率，并在建议的重试间隔后再试
    /// </remarks>
    public class RateLimitMiddleware(RequestDelegate next, ICache cache)
    {
        private readonly RequestDelegate _next = next;
        private readonly ICache _cache = cache;
        private readonly int _requestLimit = 100;
        private readonly TimeSpan _timeLimit = TimeSpan.FromSeconds(1);

        /// <summary>
        /// 中间件处理函数，当请求经过该中间件时被调用
        /// </summary>
        /// <param name="context">HTTP上下文对象，包含请求和响应的信息</param>
        /// <returns>一个Task对象，表示异步操作</returns>
        public async Task Invoke(HttpContext context)
        {
            string key = $"{context.Connection.RemoteIpAddress}:{context.Request.Path}";

            // 从缓存中获取或创建令牌桶
            var tokenBucket = await _cache.GetOrSet(key, () =>
            {
                return new TokenBucket(_requestLimit, _timeLimit);
            }, _timeLimit);

            // 尝试消耗一个令牌
            if (!tokenBucket.Consume())
            {
                // 如果没有可用的令牌，返回429状态码
                await ContextResponse.ImmediateReturn(context, 429, $"请求过多，建议您等待{_timeLimit}秒后再重新尝试。");
                return;
            }

            // 调用下一个中间件
            await _next(context);
        }
    }


    /// <summary>
    /// 表示一个令牌桶，具有最大容量和填充速率。
    /// </summary>
    /// <remarks>
    /// 使用指定的最大令牌数和填充时间初始化 TokenBucket 类的新实例。
    /// </remarks>
    /// <param name="maxTokens">桶能容纳的最大令牌数。</param>
    /// <param name="fillTime">在此时间间隔后重新填充桶中的令牌。</param>
    public class TokenBucket(int maxTokens, TimeSpan fillTime)
    {
        private readonly int _maxTokens = maxTokens;
        private readonly TimeSpan _fillTime = fillTime;
        private int _tokens = maxTokens;
        private DateTime _lastFill = DateTime.Now;
        private readonly object _syncRoot = new();

        /// <summary>
        /// 尝试从桶中消耗一个令牌
        /// </summary>
        /// <returns>如果成功消耗一个令牌，则为true；否则为false</returns>
        public bool Consume()
        {
            lock (_syncRoot)
            {
                // 如果距离上次填充时间已经过去，重新填充令牌桶
                if ((DateTime.Now - _lastFill) > _fillTime)
                {
                    _tokens = _maxTokens;
                    _lastFill = DateTime.Now;
                }

                // 如果有可用的令牌，消耗一个令牌并返回true
                if (_tokens > 0)
                {
                    _tokens--;
                    return true;
                }
                // 否则返回false
                else
                {
                    return false;
                }
            }
        }
    }
}
