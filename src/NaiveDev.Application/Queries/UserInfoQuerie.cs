using MediatR;
using NaiveDev.Application.Dtos;
using NaiveDev.Infrastructure.Commons;

namespace NaiveDev.Application.Queries
{
    /// <summary>
    /// 个人信息查询
    /// </summary>
    public class UserInfoQuerie : IRequest<ResponseBody<UserInfoResponseDto>>
    {

    }
}
