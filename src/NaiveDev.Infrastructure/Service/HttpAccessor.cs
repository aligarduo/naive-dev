using NaiveDev.Infrastructure.Auth;

namespace NaiveDev.Infrastructure.Service
{
    /// <summary>
    /// HTTP访问器类，用于在异步上下文中存储和获取请求相关的信息
    /// </summary>
    public class HttpAccessor
    {
        /// <summary>
        /// 用于存储AccessorHolder实例的异步本地变量
        /// </summary>
        private readonly static AsyncLocal<AccessorHolder> _asyncLocal = new();

        /// <summary>
        /// 用于存储CsrfHolder实例的异步本地变量（当前未使用） 
        /// </summary>
        private readonly static AsyncLocal<CsrfHolder> _csrfAsyncLocal = new();

        /// <summary>
        /// 获取或设置当前请求用户的信息
        /// </summary>
        public static Accessor? Accessor
        {
            get
            {
                return _asyncLocal.Value?.Accessor;
            }
            private set
            {
                if (_asyncLocal.Value is not null)
                {
                    _asyncLocal.Value.Accessor = null;
                }

                _asyncLocal.Value = new AccessorHolder { Accessor = value };
            }
        }

        /// <summary>
        /// 获取或设置当前的跨站点请求伪造信息
        /// </summary>
        public static string? Csrf
        {
            get
            {
                return _csrfAsyncLocal.Value?.Csrf;
            }
            private set
            {
                if (_csrfAsyncLocal.Value is not null)
                {
                    _csrfAsyncLocal.Value.Csrf = string.Empty;
                }

                _csrfAsyncLocal.Value = new CsrfHolder { Csrf = value };
            }
        }

        /// <summary>
        /// 设置Accessor属性
        /// </summary>
        /// <param name="accessor">要设置的Accessor实例</param>
        public static void SetAccessor(Accessor accessor)
        {
            if (accessor is not null)
            {
                Accessor = accessor;
            }
        }

        /// <summary>
        /// 设置Csrf属性
        /// </summary>
        /// <param name="csrf">要设置的Csrf实例</param>
        public static void SetCsrf(string csrf)
        {
            if (!string.IsNullOrWhiteSpace(csrf))
            {
                Csrf = csrf;
            }
        }

        /// <summary>
        /// 辅助类，用于存储Accessor实例
        /// 通过这个类可以管理Accessor的访问和存储，在异步上下文中使用
        /// </summary>
        private class AccessorHolder
        {
            public Accessor? Accessor;
        }

        /// <summary>
        /// 辅助类，用于存储Csrf实例
        /// 通过这个类可以管理Csrf的访问和存储，在异步上下文中使用
        /// </summary>
        private class CsrfHolder
        {
            public string? Csrf { get; set; }
        }
    }
}
