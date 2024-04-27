using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NaiveDev.Infrastructure.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NaiveDev.Infrastructure.Extensions
{
    /// <summary>   
    /// Swagger文档相关的扩展方法类，提供添加Swagger文档生成的相关服务
    /// </summary>  
    public static class SwaggerExtensions
    {
        /// <summary>
        /// 扩展方法，用于向IServiceCollection中添加Swagger文档生成服务
        /// </summary>
        /// <param name="services">IServiceCollection实例，用于注册服务</param>
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(setupAction =>
            {
                // 获取ApiVersionAttribute枚举的所有版本名称，并为每个版本生成Swagger文档
                typeof(ApiVersionAttribute).GetEnumNames().ToList().ForEach(version =>
                {
                    setupAction.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = "Naive Dev",
                        Description = "一个 .NET WebAPI 开发框架。领域驱动，开箱即用。只需要定义 Entitie 与 DTO，就可以快速完成 Restful 风格的 WebAPI 接口开发。",
                        Version = version,
                        Contact = new OpenApiContact()
                        {
                            Name = "一起养条鱼吧"
                        },
                    });
                });

                // 自定义操作的ID，格式为“控制器名-操作名”
                setupAction.CustomOperationIds(apiDesc =>
                {
                    ControllerActionDescriptor? controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                    return controllerAction?.ControllerName + "-" + controllerAction?.ActionName;
                });

                // 获取应用程序的基础目录路径
                string basePath = AppContext.BaseDirectory;
                // 遍历基础目录及其所有子目录，找到所有的XML文件，并将它们作为注释包含到Swagger文档中
                foreach (string xmlFile in Directory.GetFiles(basePath, "*.xml", SearchOption.AllDirectories))
                {
                    setupAction.IncludeXmlComments(xmlFile, true);
                }

                // 为Swagger文档添加枚举模式过滤器，用于格式化枚举类型
                setupAction.SchemaFilter<EnumSchemaFilter>();

                // 使用内联定义来表示枚举类型，而不是将它们定义为单独的定义
                setupAction.UseInlineDefinitionsForEnums();

                // 为Swagger文档添加操作过滤器，用于添加自定义扩展属性
                setupAction.OperationFilter<SwaggerOperationFilter>();
            });
        }

        /// <summary>
        /// 枚举模式过滤器，用于在OpenAPI模式中过滤和格式化枚举类型
        /// </summary>
        public class EnumSchemaFilter : ISchemaFilter
        {
            /// <summary>
            /// 应用过滤器到OpenAPI模式中
            /// </summary>  
            /// <param name="model">待处理的OpenAPI模式对象</param>
            /// <param name="context">过滤器上下文，包含有关模式的信息</param>
            public void Apply(OpenApiSchema model, SchemaFilterContext context)
            {
                // 如果上下文中的类型是枚举类型
                if (context.Type.IsEnum)
                {
                    // 清除现有的枚举值
                    model.Enum.Clear();
                    // 获取枚举类型的所有名称  
                    Enum.GetNames(context.Type).ToList().ForEach(name =>
                    {
                        // 将名称解析为枚举值
                        Enum @enum = (Enum)Enum.Parse(context.Type, name);
                        // 将枚举的名称和对应的整数值（转换为long）添加到OpenAPI模式中，并以特定格式显示
                        model.Enum.Add(new OpenApiString($"<br>{name} : {Convert.ToInt64(Enum.Parse(context.Type, name))} "));
                    });
                }
            }
        }

        /// <summary>
        /// Swagger操作过滤器，用于为OpenAPI操作添加自定义扩展属性
        /// </summary>
        public class SwaggerOperationFilter : IOperationFilter
        {
            /// <summary>
            /// 应用过滤器到OpenAPI操作中
            /// </summary>
            /// <param name="operation">待处理的OpenAPI操作对象</param>
            /// <param name="context">过滤器上下文，包含有关操作的信息</param>
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                // 获取操作方法的自定义属性SwaggerPositionAttribute的Position值
                // 如果没有找到该属性，则默认为0
                var position = GetOperationPosition(context.MethodInfo);
                // 将位置值添加到OpenAPI操作的扩展属性中，键为"x-position"
                operation.Extensions["x-position"] = new OpenApiInteger(position);
            }

            /// <summary>
            /// 获取操作方法的SwaggerPositionAttribute属性的Position值
            /// </summary>
            /// <param name="methodInfo">操作方法的MethodInfo对象</param>
            /// <returns>SwaggerPositionAttribute的Position值，如果未找到则返回0</returns>
            private static int GetOperationPosition(MethodInfo methodInfo)
            {
                // 尝试获取方法上的SwaggerPositionAttribute属性
                return methodInfo.GetCustomAttribute<SwaggerPositionAttribute>()?.Position ?? 0;
            }
        }
    }
}