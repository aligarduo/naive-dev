using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace NaiveDev.Infrastructure.Extensions
{
    /// <summary>
    /// 提供针对.NET中IServiceCollection的扩展方法，用于添加控制器服务并配置JSON选项
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// 扩展IServiceCollection，以添加控制器服务和配置JSON序列化选项
        /// </summary>
        /// <param name="services">IServiceCollection实例，用于添加服务和配置</param>
        public static void AddController(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // 设置JSON序列化时的属性命名策略为SnakeCase（下划线分隔）
                    options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    // 设置JSON序列化时属性名称不区分大小写
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });
        }
    }

    /// <summary>  
    /// 实现了JsonNamingPolicy接口的SnakeCase命名策略类，用于将驼峰命名转换为下划线命名
    /// </summary>  
    public partial class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        /// <summary>
        /// 将给定的属性名转换为SnakeCase格式（下划线分隔）
        /// </summary>
        /// <param name="name">要转换的属性名</param>
        /// <returns>转换后的SnakeCase格式的属性名</returns>
        public override string ConvertName(string name)
        {
            // 使用正则表达式替换驼峰命名中的大写字母，并在其前添加下划线
            // 然后将所有字符转换为小写，并删除字符串开头的任何下划线
            return ConvertSnakeCaseNamingPolicy().Replace(name, match => "_" + match.Value.ToLower()).TrimStart('_');
        }

        /// <summary>
        /// 生成一个正则表达式，用于匹配以大写字母开头，后面跟随小写字母或数字的命名规则
        /// </summary>
        /// <returns>返回一个编译过的Regex对象，用于匹配SnakeCase命名风格的字符串</returns>
        [GeneratedRegex("([A-Z][a-z0-9]*)", RegexOptions.Compiled)]
        private static partial Regex ConvertSnakeCaseNamingPolicy();
    }
}
