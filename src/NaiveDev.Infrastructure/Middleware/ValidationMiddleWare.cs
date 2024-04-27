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
    public class ValidationMiddleware(RequestDelegate next)
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
            // 定义一个变量originalBodyStream来保存原始的响应流，
            // 这样我们可以在处理完请求之后，如果需要的话，将内存流中的内容复制回原始的响应流中。
            var originalBodyStream = context.Response.Body;

            // 创建一个新的MemoryStream实例，用于临时存储中间件处理过程中产生的响应体内容。
            // 这样我们可以在之后检查状态码、处理异常，或者在必要时修改响应内容。
            var responseBody = new MemoryStream();

            try
            {
                // 使用内存流临时存储响应体内容
                context.Response.Body = responseBody;

                // 调用下一个中间件或请求处理逻辑
                await _next(context);

                // 检查响应状态码是否为400（Bad Request）
                if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    // 重置响应流位置到开始位置
                    responseBody.Seek(0, SeekOrigin.Begin);

                    // 读取响应体内容
                    string message = await new StreamReader(responseBody).ReadToEndAsync();

                    // 清除现有的响应内容
                    context.Response.Clear();

                    // 设置新的响应
                    await ContextResponse.ImmediateReturn(context, StatusCodes.Status400BadRequest, ConvertJsonFormat(message));

                    // 立即返回，不再执行后续代码
                    return;
                }
            }
            catch (Exception ex)
            {
                // 如果发生异常，将异常信息作为500响应返回
                await ContextResponse.ImmediateReturn(context, StatusCodes.Status500InternalServerError, ex.Message);

                // 立即返回，不再执行后续代码
                return;
            }
            finally
            {
                // 只有在没有调用ImmediateReturn时才将响应流重置回原始流
                if (context.Response.Body != originalBodyStream)
                {
                    // 将内存流中的响应内容复制回原始响应流
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);

                    // 重置响应流的位置到开始位置
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    // 将响应流重置回原始流
                    context.Response.Body = originalBodyStream;
                }
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
