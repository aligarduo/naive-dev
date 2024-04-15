using System.ComponentModel.DataAnnotations;
using MediatR;
using NaiveDev.Infrastructure.Commons;

namespace NaiveDev.Application.Commands
{
    /// <summary>
    /// 注册命令
    /// </summary>
    public class SignUpCommand : IRequest<ResponseBody>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不可为空")]
        [MaxLength(20, ErrorMessage = "输入的用户名超出长度限制")]
        public string? Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [MaxLength(64, ErrorMessage = "输入的密码超出长度限制")]
        public string? Password { get; set; }
    }
}
