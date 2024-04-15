using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace NaiveDev.Infrastructure.Commons
{
    /// <summary>
    /// 响应正文
    /// </summary>
    public class ResponseBody
    {
        /// <summary>
        /// 状态码
        /// </summary>
        [JsonProperty(PropertyName = "code", Order = 1)]
        [JsonPropertyName("code")]
        public virtual int Code { get; init; }

        /// <summary>
        /// 状态信息
        /// </summary>
        [JsonProperty(PropertyName = "message", Order = 2)]
        [JsonPropertyName("message")]
        public virtual string? Message { get; init; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public static ResponseBody Succeed() => new()
        {
            Code = 0,
            Message = "成功"
        };

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">状态信息</param>
        /// <returns></returns>
        public static ResponseBody Fail(int code, string message) => new()
        {
            Code = code,
            Message = message
        };
    }

    /// <summary>
    /// 泛型响应正文
    /// </summary>
    /// <typeparam name="T">响应数据类型</typeparam>
    public class ResponseBody<T> : ResponseBody
    {
        /// <summary>
        /// 数据负载
        /// </summary>
        [JsonProperty(PropertyName = "data", Order = 3, NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("data")]
        public virtual T? Data { get; init; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">数据负载</param>
        /// <returns></returns>
        public static ResponseBody<T> Succeed(T data) => new()
        {
            Code = 0,
            Message = "成功",
            Data = data
        };

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">状态信息</param>
        /// <returns></returns>
        public static new ResponseBody<T> Fail(int code, string message) => new()
        {
            Code = code,
            Message = message,
            Data = default
        };
    }

    /// <summary>
    /// 泛型响应正文分页
    /// </summary>
    /// <typeparam name="T">响应数据类型</typeparam>
    public class ResponseBodyPage<T> : ResponseBody<T>
    {
        /// <summary>
        /// 这是第几页
        /// </summary>
        [JsonProperty(PropertyName = "page_number", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("page_number")]
        public virtual int? PageNumber { get; init; }

        /// <summary>
        /// 一页有几条数据
        /// </summary>
        [JsonProperty(PropertyName = "PropertyName", Order = 5, NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("page_size")]
        public virtual int? PageSize { get; init; }

        /// <summary>
        /// 全部有多少行
        /// </summary>
        [JsonProperty(PropertyName = "total_number", Order = 6, NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("total_number")]
        public virtual int? TotalNumber { get; init; }

        /// <summary>
        /// 全部有多少页
        /// </summary>
        [JsonProperty(PropertyName = "total_page", Order = 7, NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("total_page")]
        public virtual int? TotalPage { get; init; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">数据负载</param>
        /// <param name="pageNumber">这是第几页</param>
        /// <param name="pageSize">一页有几条数据</param>
        /// <param name="totalNumber">全部有多少行</param>
        /// <param name="totalPage">全部有多少页</param>
        /// <returns></returns>
        public ResponseBodyPage<T> Succeed(T data, int pageNumber, int pageSize, int totalNumber, int totalPage) => new()
        {
            Code = 0,
            Message = "成功",
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalNumber = totalNumber,
            TotalPage = totalPage
        };

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">状态信息</param>
        /// <returns></returns>
        public static new ResponseBodyPage<T> Fail(int code, string message) => new()
        {
            Code = code,
            Message = message,
        };
    }
}