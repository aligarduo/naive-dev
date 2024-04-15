using NaiveDev.Infrastructure.Auth;

namespace NaiveDev.Infrastructure.Service
{
    /// <summary>
    /// 接口实现基类
    /// </summary>
    public abstract class ServiceBase
    {
        /// <summary>
        /// 当前请求用户信息
        /// </summary>
        public static Accessor Accessor => HttpAccessor.Accessor ?? new Accessor();

        /// <summary>
        /// 跨站点请求伪造
        /// </summary>
        public static string Csrf => HttpAccessor.Csrf ?? string.Empty;
    }
}
