using System.ComponentModel.DataAnnotations;
using MediatR;
using NaiveDev.Application.Dtos;
using NaiveDev.Infrastructure.Commons;

namespace NaiveDev.Application.Commands
{
    /// <summary>
    /// 续约命令
    /// </summary>
    public class RenewalCommand : IRequest<ResponseBody<RenewalResponseDto>>
    {
        /// <summary>
        /// 续签Token
        /// </summary>
        [Required(ErrorMessage = "RefreshToken不可为空")]
        [MaxLength(1024)]
        public string? RefreshToken { get; set; }
    }
}
