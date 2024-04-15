using IGeekFan.AspNetCore.Knife4jUI;

using Microsoft.AspNetCore.Builder;

using NaiveDev.Infrastructure.Attributes;

namespace NaiveDev.Infrastructure.Middleware
{
    /// <summary>
    /// Swagger中间件扩展类，用于在.NET应用程序中集成Swagger和Knife4UI文档界面
    /// </summary>
    public static class SwaggerMiddleWare
    {
        /// <summary>
        /// 扩展方法，用于在应用构建器（IApplicationBuilder）上添加Swagger和Knife4UI中间件
        /// </summary>
        /// <param name="app">.NET的应用构建器实例</param>
        public static void UseSwaggers(this IApplicationBuilder app)
        {
            // 添加Swagger中间件，用于提供Swagger API文档的JSON格式数据
            app.UseSwagger();

            // 添加Knife4UI中间件，用于提供API文档的Web界面
            // 配置Knife4UI选项的委托
            app.UseKnife4UI(setupAction =>
            {
                // 设置Knife4UI的基础路由前缀为空字符串，表示不添加额外的路由前缀
                setupAction.RoutePrefix = "";

                // 获取所有可用的API版本枚举名称，按名称顺序排列
                var versions = typeof(ApiVersionAttribute).GetEnumNames().OrderBy(e => e).ToList();

                // 遍历每个API版本，为Knife4UI配置Swagger文档的访问端点
                // 当前处理的API版本
                versions.ForEach(version =>
                {
                    // 配置Knife4UI的Swagger文档端点，指定Swagger JSON文件的路径和文档显示的标题
                    setupAction.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{version}");
                });
            });
        }
    }
}
