using System.ComponentModel.DataAnnotations;
using MediatR;
using NaiveDev.Application.Dtos;
using NaiveDev.Infrastructure.Commons;

namespace NaiveDev.Application.Commands
{
    /// <summary>
    /// 登录命令
    /// </summary>
    public class SignInCommand : IRequest<ResponseBody<SignInResponseDto>>
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(ErrorMessage = "账号不可为空")]
        [MaxLength(20, ErrorMessage = "输入的账号超出长度限制")]
        public string? Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [MaxLength(64, ErrorMessage = "输入的密码超出长度限制")]
        public string? Password { get; set; }
    }
}
