using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace NaiveDev.Infrastructure.Attributes
{
    /// <summary>
    /// 自定义路由
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomRouteAttribute(ApiVersionAttribute version) : RouteAttribute($"/api/{version}/[controller]"), IApiDescriptionGroupNameProvider
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; } = version.ToString();
    }
}