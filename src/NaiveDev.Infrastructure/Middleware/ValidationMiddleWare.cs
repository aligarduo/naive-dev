using Microsoft.AspNetCore.Http;

using NaiveDev.Infrastructure.Commons;
using NaiveDev.Infrastructure.Tools;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NaiveDev.Infrastructure.Middleware
{
    /// <summary>
    /// 验证中间件类，用于在.NET请求处理管道中执行自定义验证逻辑
    /// </summary>
    public class ValidationMiddleWare(RequestDelegate next)
    {
        /// <summary>
        /// 请求委托，表示在验证中间件之后的请求处理逻辑
        /// </summary>
        private readonly RequestDelegate _next = next;

        /// <summary>
        /// 执行验证中间件逻辑
        /// </summary>
        /// <param name="context">当前HTTP请求的上下文对象</param>
        /// <returns>异步操作的任务</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                var originalBodyStream = context.Response.Body;

                // 使用内存流临时存储响应体内容，以便后续处理
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // 调用下一个中间件或请求处理逻辑
                await _next(context);

                // 检查响应状态码是否为400（Bad Request）
                if (context.Response.StatusCode == 400)
                {
                    // 重置响应流的位置到开始位置，以便读取内容
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    // 读取响应体内容
                    string message = await new StreamReader(context.Response.Body).ReadToEndAsync();

                    // 重置响应流的位置到开始位置，以便后续写入内容
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    // 清除现有的响应内容
                    context.Response.Clear();

                    // 设置新的响应状态码和类型
                    await ContextResponse.ImmediateReturn(context, 400, ConvertJsonFormat(message));
                }
                else if (context.Response.StatusCode == 404)
                {
                    // 设置新的响应状态码和类型
                    await ContextResponse.ImmediateReturn(context, 404, "404 not found");
                }

                // 将内存流中的响应内容复制回原始响应流
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                await ContextResponse.ImmediateReturn(context, 500, ex.Message);
            }
        }

        /// <summary>
        /// 将输入的JSON字符串解析为特定的错误消息格式
        /// </summary>
        /// <param name="inputJson">待解析的JSON字符串</param>
        /// <returns>解析后的错误消息，如果无法解析则返回"错误无法解析"</returns>
        public static string ConvertJsonFormat(string inputJson)
        {
            try
            {
                // 解析输入的JSON字符串
                JObject parsedJson = JObject.Parse(inputJson);

                // 尝试从JSON对象中获取"errors"字段下的第一个错误消息
                string? errorMessage = parsedJson["errors"]?
                    .Children()
                    .FirstOrDefault()?
                    .Children()
                    .FirstOrDefault()?
                    .ToString();

                // 将错误消息转换为字符串数组，并获取第一个元素
                errorMessage = errorMessage?.To<string[]>().FirstOrDefault();

                // 返回解析到的错误消息，如果没有找到则返回"错误无法解析"
                return errorMessage ?? "错误无法解析";
            }
            catch (JsonReaderException)
            {
                // 如果解析JSON时发生异常，则返回"错误无法解析"
                return "错误无法解析";
            }
        }
    }
}
