using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.Extensions.Logging;
using NaiveDev.Application.Commands;
using NaiveDev.Application.Dtos;
using NaiveDev.Domain.Entities;
using NaiveDev.Domain.ValueObjects;
using NaiveDev.Infrastructure.Caches;
using NaiveDev.Infrastructure.Commons;
using NaiveDev.Infrastructure.Enums;
using NaiveDev.Infrastructure.Jwt;
using NaiveDev.Infrastructure.Persistence;
using NaiveDev.Infrastructure.Service;
using NaiveDev.Infrastructure.Tools;

namespace NaiveDev.Application.CommandHandlers
{
    /// <summary>
    /// 通行证命令处理器
    /// </summary>
    public class PassPortCommandHandler(ILogger<PassPortCommandHandler> logger, ICache cache, IRepository<User> user) : ServiceBase,
        IRequestHandler<SignInCommand, ResponseBody<SignInResponseDto>>,
        IRequestHandler<SignUpCommand, ResponseBody>,
        IRequestHandler<RenewalCommand, ResponseBody<RenewalResponseDto>>,
        IRequestHandler<SignOutCommand, ResponseBody>
    {
        private readonly ILogger<PassPortCommandHandler> _logger = logger;
        private readonly ICache _cache = cache;
        private readonly IRepository<User> _user = user;

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseBody<SignInResponseDto>> Handle(SignInCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.Queryable().FirstAsync(q => q.Account == command.Account, cancellationToken);
                if (user is null)
                {
                    return ResponseBody<SignInResponseDto>.Fail(201, "账号不存在");
                }
                if (!PasswordHash.VerifyPassword(user.Password, command.Password, user.Salt))
                {
                    return ResponseBody<SignInResponseDto>.Fail(201, "密码错误");
                }

                string guid = Guid.NewGuid().ToString();

                string AccessToken = await JwtTokenProvider.BuildTokenAsync(JwtType.AccessToken, guid, DateTime.Now.AddHours(2));
                string RefreshToken = await JwtTokenProvider.BuildTokenAsync(JwtType.RefreshToken, guid, DateTime.Now.AddDays(7));

                await _cache.SetCacheAsync($"{guid}:/{JwtType.AccessToken.Description()}", user, TimeSpan.FromHours(2), cancellationToken);
                await _cache.SetCacheAsync($"{guid}:/{JwtType.RefreshToken.Description()}", user, TimeSpan.FromDays(7), cancellationToken);

                var result = new SignInResponseDto() { AccessToken = AccessToken, ExpiresIn = 7200, RefreshToken = RefreshToken };

                return ResponseBody<SignInResponseDto>.Succeed(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignIn");
                return ResponseBody<SignInResponseDto>.Fail(500, ex.Message);
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseBody> Handle(SignUpCommand command, CancellationToken cancellationToken)
        {
            try
            {
                bool user = await _user.Queryable().AnyAsync(q => q.Name.Equals(command.Name), cancellationToken);
                if (user)
                {
                    return ResponseBody.Fail(201, "用户名已存在");
                }

                await _user.Insertable(User.Create(command.Name, command.Password)).ExecuteCommandAsync();

                return ResponseBody.Succeed();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignUp");
                return ResponseBody.Fail(500, ex.Message);
            }
        }

        /// <summary>
        /// 续约
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseBody<RenewalResponseDto>> Handle(RenewalCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (!JwtTokenProvider.ValidateTokenAsync(command.RefreshToken ?? "", out JwtSecurityToken? jwtSecurityToken))
                {
                    return ResponseBody<RenewalResponseDto>.Fail(201, "无效的TOKEN");
                }

                // 提取令牌中的声明信息  
                List<Claim>? claimList = jwtSecurityToken?.Claims.ToList();

                // 尝试从声明列表中获取type和csrf字段的值  
                string? type = claimList?.Where(q => q.Type == "type").FirstOrDefault()?.Value;
                string? csrf = claimList?.Where(q => q.Type == "csrf").FirstOrDefault()?.Value;

                if (type is null || csrf is null)
                {
                    return ResponseBody<RenewalResponseDto>.Fail(201, "无效的TOKEN");
                }

                if (!type.Equals(JwtType.RefreshToken.Description()))
                {
                    return ResponseBody<RenewalResponseDto>.Fail(201, "无效的TOKEN");
                }

                var user = await _cache.GetCacheAsync<User>($"{csrf}/{JwtType.RefreshToken.Description()}", cancellationToken);
                if (user is null)
                {
                    return ResponseBody<RenewalResponseDto>.Fail(201, "无效的TOKEN");
                }

                await _cache.RemoveCacheAsync($"{csrf}:/{JwtType.AccessToken.Description()}", cancellationToken);
                await _cache.RemoveCacheAsync($"{csrf}:/{JwtType.RefreshToken.Description()}", cancellationToken);

                string guid = Guid.NewGuid().ToString();

                string AccessToken = await JwtTokenProvider.BuildTokenAsync(JwtType.AccessToken, guid, DateTime.Now.AddHours(2));
                string RefreshToken = await JwtTokenProvider.BuildTokenAsync(JwtType.RefreshToken, guid, DateTime.Now.AddDays(7));

                await _cache.SetCacheAsync($"{guid}:/{JwtType.AccessToken.Description()}", user, TimeSpan.FromHours(2), cancellationToken);
                await _cache.SetCacheAsync($"{guid}:/{JwtType.RefreshToken.Description()}", user, TimeSpan.FromDays(7), cancellationToken);

                var result = new RenewalResponseDto() { AccessToken = AccessToken, ExpiresIn = 7200, RefreshToken = RefreshToken };

                return ResponseBody<RenewalResponseDto>.Succeed(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Renewal");
                return ResponseBody<RenewalResponseDto>.Fail(500, ex.Message);
            }
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseBody> Handle(SignOutCommand command, CancellationToken cancellationToken)
        {
            await _cache.RemoveCacheAsync($"{Csrf}:/{JwtType.AccessToken.Description()}", cancellationToken);
            await _cache.RemoveCacheAsync($"{Csrf}:/{JwtType.RefreshToken.Description()}", cancellationToken);

            return ResponseBody.Succeed();
        }
    }
}