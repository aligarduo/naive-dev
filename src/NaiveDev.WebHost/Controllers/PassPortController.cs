using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaiveDev.Application.Commands;
using NaiveDev.Application.Dtos;
using NaiveDev.Application.Queries;
using NaiveDev.Infrastructure.Attributes;
using NaiveDev.Infrastructure.Commons;

namespace NaiveDev.WebHost.Controllers
{
    /// <summary>
    /// 通行证
    /// </summary>
    [CustomRoute(ApiVersionAttribute.v1)]
    [ApiController]
    public class PassPortController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("signin")]
        [SwaggerPosition(1)]
        public async Task<ResponseBody<SignInResponseDto>> SignInAsync([FromBody] SignInCommand command)
        {
            return await _mediator.Send(command);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="command">创建用户命令</param>
        /// <returns></returns>
        [HttpPost]
        [Route("signup")]
        [SwaggerPosition(2)]
        public async Task<ResponseBody> SignUpAsync([FromBody] SignUpCommand command)
        {
            return await _mediator.Send(command);
        }

        /// <summary>
        /// 续约
        /// </summary>
        /// <param name="command">续约命令</param>
        /// <returns></returns>
        [HttpPost]
        [Route("renewal")]
        [SwaggerPosition(3)]
        public async Task<ResponseBody<RenewalResponseDto>> RenewalAsync([FromBody] RenewalCommand command)
        {
            return await _mediator.Send(command);
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("signout")]
        [SwaggerPosition(4)]
        public async Task<ResponseBody> SignOutAsync()
        {
            return await _mediator.Send(new SignOutCommand());
        }

        /// <summary>
        /// 个人信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("userinfo")]
        [SwaggerPosition(5)]
        public async Task<ResponseBody<UserInfoResponseDto>> UserInfoAsync()
        {
            return await _mediator.Send(new UserInfoQuerie());
        }
    }
}
