using System.Security.Cryptography;
using NaiveDev.Infrastructure.Entities;

namespace NaiveDev.Domain.ValueObjects
{
    /// <summary>  
    /// 密码盐值对象  
    /// </summary>  
    public class PasswordSalt : ValueObjectBase
    {
        /// <summary>  
        /// 盐值
        /// </summary>  
        public string SaltValue { get; }

        /// <summary>  
        /// 私有构造函数，确保对象的不变性  
        /// </summary>  
        private PasswordSalt(string saltValue)
        {
            SaltValue = saltValue;
        }

        /// <summary>  
        /// 静态工厂方法，用于生成新的盐值对象  
        /// </summary>  
        /// <param name="size">盐值的大小（以字节为单位）</param>  
        /// <returns>一个新的盐值对象</returns>  
        public static PasswordSalt Create(int size = 32)
        {
            using var rng = RandomNumberGenerator.Create();
            var saltValue = new byte[size];
            rng.GetBytes(saltValue);

            // 将盐值从字节数组转换为字符串
            string saltString = Convert.ToBase64String(saltValue);
            return new PasswordSalt(saltString);
        }
    }
}
