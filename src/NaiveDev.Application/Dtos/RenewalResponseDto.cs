namespace NaiveDev.Application.Dtos
{
    /// <summary>
    /// 续约响应数据传输对象
    /// </summary>
    public class RenewalResponseDto
    {
        /// <summary>
        /// 授权令牌
        /// </summary>
        public required string AccessToken { get; init; }

        /// <summary>
        /// 该access_token的有效期，单位为秒
        /// </summary>
        public required int ExpiresIn { get; init; }

        /// <summary>
        /// 在授权自动续期步骤中，获取新的Access_Token时需要提供的参数。
        /// 每次生成最新的refresh_token，且仅一次有效，一次登录，refresh_token整个续票过程，最长有效期：7天
        /// </summary>
        public required string RefreshToken { get; init; }
    }
}
