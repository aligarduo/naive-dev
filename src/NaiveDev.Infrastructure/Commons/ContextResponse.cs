using Microsoft.AspNetCore.Http;
using NaiveDev.Infrastructure.Tools;

namespace NaiveDev.Infrastructure.Commons
{
    /// <summary>
    /// 上下文响应
    /// </summary>
    public class ContextResponse
    {
        /// <summary>
        /// 即刻返回
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="code">状态码</param>
        /// <param name="message">描述</param>
        /// <param name="type">响应类型</param>
        /// <returns></returns>
        public static async Task ImmediateReturn(HttpContext context, int code, string message, string type = "application/json")
        {
            context.Response.StatusCode = code;
            context.Response.ContentType = type;
            await context.Response.WriteAsync(ResponseBody.Fail(code, message).ToJson());
        }
    }
}
