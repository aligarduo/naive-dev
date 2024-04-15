using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NaiveDev.Infrastructure.Jwt
{
    /// <summary>
    /// Jwt选项
    /// </summary>
    public class JwtOption
    {
        /// <summary>
        /// 受众
        /// </summary>
        public required string Audience { get; set; }

        /// <summary>
        /// 签发方
        /// </summary>
        public required string Issuer { get; set; }

        /// <summary>
        /// 安全密钥
        /// </summary>
        public required string SecurityKey { get; set; }

        /// <summary>
        /// 获取默认的Options
        /// </summary>
        /// <returns></returns>
        public static JwtOption GetDefaultOptions()
        {
            return new JwtOption
            {
                Audience = "Everyone",
                Issuer = "NaiveDev",
                SecurityKey = "0a2b0aeb3edd41789f0fd48e4c36dfba",
            };
        }

        /// <summary>
        /// 获取默认的Token验证参数
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static TokenValidationParameters GetDefaultTokenValidationParameters(JwtOption? option = null)
        {
            option ??= GetDefaultOptions();

            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = option.Issuer,
                ValidAudience = option.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(option.SecurityKey)),
                ClockSkew = TimeSpan.FromSeconds(10)
            };
        }
    }
}
