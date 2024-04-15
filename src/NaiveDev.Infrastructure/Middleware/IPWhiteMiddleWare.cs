using Microsoft.AspNetCore.Http;

using NaiveDev.Infrastructure.Commons;

namespace NaiveDev.Infrastructure.Middleware
{
    /// <summary>
    /// IP白名单中间件，用于仅允许白名单中IP地址的访问请求
    /// </summary>  
    public class IPWhiteMiddleWare(RequestDelegate next)
    {
        /// <summary>
        /// 请求处理委托的下一个中间件
        /// </summary>
        private readonly RequestDelegate _next = next;

        /// <summary>
        /// 白名单IP地址集合
        /// </summary>
        private readonly HashSet<string> _whitelistedIPs = ["::1", "127.0.0.1"];

        /// <summary>  
        /// 中间件处理函数，当请求经过该中间件时被调用。  
        /// </summary>  
        /// <param name="context">HTTP上下文对象，包含请求和响应的信息。</param>  
        /// <returns>一个Task对象，表示异步操作。</returns>  
        public async Task Invoke(HttpContext context)
        {
            // 获取远程IP地址字符串
            var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "";

            // 检查远程IP地址是否不在白名单中
            if (!_whitelistedIPs.Contains(remoteIpAddress))
            {
                // 如果IP地址不在白名单中，返回403 Forbidden状态码
                await ContextResponse.ImmediateReturn(context, 403, $"Add the IP address to the whitelist first");
                return;
            }

            // 如果IP地址在白名单中，则调用下一个请求处理委托
            await _next(context);
        }
    }
}
