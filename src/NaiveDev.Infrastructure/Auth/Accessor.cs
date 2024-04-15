namespace NaiveDev.Infrastructure.Auth
{
    /// <summary>
    /// 当前请求的用户信息
    /// </summary>
    public class Accessor
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string? Name { get; set; }
    }
}
