using System.ComponentModel;

namespace NaiveDev.Infrastructure.Enums
{
    /// <summary>
    /// Jwt类型
    /// </summary>
    public enum JwtType
    {
        /// <summary>
        /// AccessToken
        /// </summary>
        [Description("access_token")]
        AccessToken,

        /// <summary>
        /// RefreshToken
        /// </summary>
        [Description("refresh_token")]
        RefreshToken,
    }
}
