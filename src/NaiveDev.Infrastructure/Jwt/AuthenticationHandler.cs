using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NaiveDev.Infrastructure.Commons;
using NaiveDev.Infrastructure.Tools;

namespace NaiveDev.Infrastructure.Jwt
{
    /// <summary>
    /// 认证处理程序的构造函数，用于初始化认证处理程序并注入必要的依赖项
    /// </summary>
    /// <param name="options">认证方案选项的监视器，用于获取和监听认证方案选项的更改</param>
    /// <param name="logger">日志记录器工厂，用于创建日志记录器实例，用于记录认证过程中的日志信息</param>
    /// <param name="encoder">URL编码器，用于对URL中的特殊字符进行编码，以确保其正确性和安全性</param>
    public class AuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        /// <summary>
        /// 重写异步方法，处理身份验证过程
        /// </summary>
        /// <returns>身份验证结果</returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 重写处理身份验证的异步方法
        /// 当需要用户进行身份验证时，会调用此方法
        /// </summary>
        /// <param name="properties">包含有关身份验证挑战的附加信息的属性对象</param> 
        /// <returns>异步任务。</returns>  
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // 调用AuthFailureMessage方法，发送401未授权访问的响应，并附带“登录已过期”的提示信息
            await AuthFailureMessage(401, "登录已过期");
        }

        /// <summary>
        /// 重写处理禁止访问请求的异步方法
        /// 当用户没有权限访问受保护的资源时，会调用此方法
        /// </summary>
        /// <param name="properties">包含有关认证请求的附加信息的属性对象</param>
        /// <returns>异步任务</returns>
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            // 调用AuthFailureMessage方法，发送403禁止访问的响应，并附带“没有权限访问”的提示信息。  
            await AuthFailureMessage(403, "没有权限访问");
        }

        /// <summary>
        /// 发送认证失败的消息给客户端
        /// </summary>
        /// <param name="code">HTTP状态码，表示错误的类型</param>
        /// <param name="message">描述认证失败的详细信息</param>
        /// <returns>异步任务。</returns>  
        private async Task AuthFailureMessage(int code, string message)
        {
            // 设置响应的内容类型为JSON  
            Response.ContentType = "application/json";

            // 设置响应的HTTP状态码  
            Response.StatusCode = code;

            // 将认证失败的结果转换为JSON字符串，并异步写入响应流  
            await Response.WriteAsync(ResponseBody.Fail(code, message).ToJson());
        }
    }
}