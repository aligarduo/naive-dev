using MediatR;

using Microsoft.Extensions.Logging;

using NaiveDev.Application.Dtos;
using NaiveDev.Application.Queries;
using NaiveDev.Domain.Entities;
using NaiveDev.Infrastructure.Commons;
using NaiveDev.Infrastructure.Persistence;
using NaiveDev.Infrastructure.Service;
using NaiveDev.Infrastructure.Tools;

namespace NaiveDev.Application.QuerieHandlers
{
    /// <summary>
    /// 通行证查询处理器
    /// </summary>
    public class PassPortQuerieHandler(ILogger<PassPortQuerieHandler> logger, IRepository<User> user) : ServiceBase,
        IRequestHandler<UserInfoQuerie, ResponseBody<UserInfoResponseDto>>
    {
        private readonly ILogger<PassPortQuerieHandler> _logger = logger;
        private readonly IRepository<User> _user = user;

        /// <summary>
        /// 个人信息
        /// </summary>
        /// <param name="querie"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseBody<UserInfoResponseDto>> Handle(UserInfoQuerie querie, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.Queryable().FirstAsync(q => q.Id == Accessor.Id, cancellationToken);
                if (user is null)
                {
                    return ResponseBody<UserInfoResponseDto>.Fail(404, "用户不存在");
                }

                var result = user.To<UserInfoResponseDto>();
                return ResponseBody<UserInfoResponseDto>.Succeed(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserInfo");
                return ResponseBody<UserInfoResponseDto>.Fail(500, ex.Message);
            }
        }
    }
}
