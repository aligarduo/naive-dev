using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NaiveDev.Infrastructure.Enums;
using NaiveDev.Infrastructure.Tools;

namespace NaiveDev.Infrastructure.Jwt
{
    /// <summary>
    /// Jwt令牌提供程序
    /// </summary>
    public class JwtTokenProvider
    {
        private static readonly JwtOption _options;

        static JwtTokenProvider()
        {
            _options = JwtOption.GetDefaultOptions();
        }

        /// <summary>
        /// 构建令牌
        /// </summary>
        /// <param name="type">令牌类型</param>
        /// <param name="csrf">跨站点请求伪造标识</param>
        /// <param name="expires">有效期</param>
        /// <returns>一个JWT令牌</returns>
        public static async Task<string> BuildTokenAsync(JwtType type, string csrf, DateTime expires)
        {
            return await Task.Run(() =>
            {
                IEnumerable<Claim> claims = [new Claim("type", type.Description()), new Claim("csrf", csrf)];

                JwtSecurityTokenHandler tokenHandler = new();
                byte[] key = Encoding.ASCII.GetBytes(_options.SecurityKey);
                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Subject = new ClaimsIdentity(claims),
                    Issuer = _options.Issuer,
                    Audience = _options.Audience,
                    Expires = expires,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            });
        }

        /// <summary>
        /// 验证JWT令牌的有效性
        /// </summary>
        /// <param name="token">待验证的JWT令牌字符串</param>
        /// <param name="jwtSecurityToken">如果验证成功，返回解析后的JwtSecurityToken对象；否则为null</param>
        /// <returns>如果令牌验证通过，返回true；否则返回false</returns>
        public static bool ValidateTokenAsync(string token, out JwtSecurityToken? jwtSecurityToken)
        {
            // 初始化JWT令牌处理器
            var tokenHandler = new JwtSecurityTokenHandler();

            // 将安全密钥转换为字节数组
            byte[] key = Encoding.ASCII.GetBytes(_options.SecurityKey);
            try
            {
                // 验证令牌，并设置验证参数
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _options.Issuer,
                    ValidAudience = _options.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                // 确保解析的令牌是JwtSecurityToken类型
                jwtSecurityToken = validatedToken as JwtSecurityToken;

                // 如果jwtSecurityToken不为null，则验证通过
                return jwtSecurityToken != null;
            }
            catch (Exception)
            {
                // 出现异常时，验证失败，将jwtSecurityToken设置为null
                jwtSecurityToken = null;
                return false;
            }
        }
    }
}