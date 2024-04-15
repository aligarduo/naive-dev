using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace NaiveDev.Infrastructure.Jwt
{
    /// <summary>
    /// JWT相关的扩展方法类
    /// </summary>
    public static class JwtExtensions
    {
        /// <summary>
        /// 添加Jwt鉴权
        /// </summary>
        /// <param name="services"></param>
        public static void AddJwt(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = nameof(AuthenticationHandler);
                options.DefaultForbidScheme = nameof(AuthenticationHandler);
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtOption.GetDefaultTokenValidationParameters();
            }).AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>(nameof(AuthenticationHandler), options => { });
        }
    }
}