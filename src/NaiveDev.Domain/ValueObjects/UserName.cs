using System.Text.RegularExpressions;
using NaiveDev.Infrastructure.Entities;

namespace NaiveDev.Domain.ValueObjects
{
    /// <summary>
    /// 用户名值对象
    /// </summary>
    public class UserName : ValueObjectBase
    {
        /// <summary>  
        /// 用户名的值  
        /// </summary>  
        public string Value { get; }

        /// <summary>  
        /// 私有构造函数，确保对象的不变性  
        /// </summary>  
        private UserName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("用户名不能为空、空或空格", nameof(value));
            }

            if (ContainsSpecialCharacters(value))
            {
                throw new ArgumentException("用户名不能包含特殊字符。", nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// 静态工厂方法，用于创建 Username 实例
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UserName Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("用户名不能为空、空或空格", nameof(value));
            }

            return new UserName(value);
        }

        /// <summary>
        /// 检查字符串是否包含特殊字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static bool ContainsSpecialCharacters(string input)
        {
            Regex specialCharactersRegex = new("[^a-zA-Z0-9]");
            return specialCharactersRegex.IsMatch(input);
        }
    }
}
