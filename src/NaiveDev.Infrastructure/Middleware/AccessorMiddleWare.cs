using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NaiveDev.Infrastructure.Auth;
using NaiveDev.Infrastructure.Caches;
using NaiveDev.Infrastructure.Commons;
using NaiveDev.Infrastructure.Service;
using NaiveDev.Infrastructure.Tools;

namespace NaiveDev.Infrastructure.Middleware
{
    /// <summary>
    /// 缓存当前请求的用户信息中间件
    /// </summary>
    public class AccessorMiddleWare(RequestDelegate next, ICache cache)
    {
        private readonly RequestDelegate _next = next;
        private readonly ICache _cache = cache;

        /// <summary>
        /// 中间件处理函数，当请求经过该中间件时被调用
        /// </summary>
        /// <param name="context">HTTP上下文对象，包含请求和响应的信息</param>
        /// <returns>一个Task对象，表示异步操作</returns>
        public async Task Invoke(HttpContext context)
        {
            Endpoint? endpoint = context.GetEndpoint();
            if (endpoint is not null)
            {
                AuthorizeAttribute? authAttribute = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
                if (authAttribute is not null)
                {
                    // 提取令牌中的声明信息
                    List<Claim> claimList = context.User.Claims.ToList();

                    // 尝试从声明列表中获取csrf和type字段的值
                    string? csrf = claimList.Where(q => q.Type == "csrf").FirstOrDefault()?.Value;
                    string? type = claimList.Where(q => q.Type == "type").FirstOrDefault()?.Value;

                    if (!string.IsNullOrWhiteSpace(csrf) && !string.IsNullOrWhiteSpace(type))
                    {
                        // 根据csrf和type从缓存中获取用户信息
                        string userString = await _cache.GetCacheAsync($"{csrf}:/{type}");
                        if (string.IsNullOrEmpty(userString))
                        {
                            await ContextResponse.ImmediateReturn(context, 401, "登录已过期");
                            return;
                        }

                        Accessor accessor = userString.To<Accessor>();
                        HttpAccessor.SetAccessor(accessor);
                        HttpAccessor.SetCsrf(csrf);
                    }
                }
            }

            await _next(context);
        }
    }
}
