namespace NaiveDev.Infrastructure.Commons
{
    /// <summary>
    /// 外部服务响应
    /// </summary>
    public class ExternalResponse<T>
    {
        /// <summary>
        /// 成功
        /// </summary>
        public virtual bool IsSucceed => Code == 200;

        /// <summary>
        /// 状态码
        /// </summary>
        public virtual int Code { get; init; }

        /// <summary>
        /// 响应数据
        /// </summary>
        public virtual T? Data { get; init; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public virtual string? Error { get; init; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public static ExternalResponse<T> Succeed(int code, object data) => new()
        {
            Code = code,
            Data = (T)data
        };

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">状态信息</param>
        /// <returns></returns>
        public static ExternalResponse<T> Fail(int code, string message) => new()
        {
            Code = code,
            Error = message
        };
    }
}