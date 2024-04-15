namespace NaiveDev.Application.Dtos
{
    /// <summary>
    /// 用户信息响应数据传输对象
    /// </summary>
    public class UserInfoResponseDto
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string? Account { get; init; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string? Name { get; init; }
    }
}
