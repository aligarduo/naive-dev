using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NaiveDev.Infrastructure.Entities;

namespace NaiveDev.Domain.ValueObjects
{
    /// <summary>  
    /// 密码哈希值对象  
    /// </summary>  
    public class PasswordHash : ValueObjectBase
    {
        /// <summary>  
        /// 哈希后的密码值  
        /// </summary>  
        public string HashValue { get; }

        /// <summary>  
        /// 私有构造函数，确保对象的不变性  
        /// </summary>  
        private PasswordHash(string hashedValue)
        {
            HashValue = hashedValue;
        }

        /// <summary>  
        /// 静态工厂方法，用于创建 PasswordHash 实例  
        /// </summary>  
        /// <param name="password">要哈希的密码</param>  
        /// <param name="salt">用于哈希的盐值</param>  
        /// <returns>密码哈希值对象</returns>  

        public static PasswordHash Create(string? password, string salt)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("密码不能为空、空或空格", nameof(password));
            }

            // 将盐值从字符串转换为字节数组
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            // 使用盐值哈希密码  
            byte[] hashedBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            string hashedValue = Convert.ToBase64String(hashedBytes);

            return new PasswordHash(hashedValue);
        }

        /// <summary>  
        /// 检查提供的密码是否匹配存储的哈希值  
        /// </summary>  
        /// <param name="hashed">哈希密码</param>  
        /// <param name="password">要检查的密码</param>  
        /// <param name="salt">用于哈希的盐值</param>  
        /// <returns>如果密码匹配则返回true，否则返回false</returns>  
        public static bool VerifyPassword(string hashed, string? password, string salt)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("密码不能为空、空或空格", nameof(password));
            }

            // 将盐值从字符串转换为字节数组
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            // 使用盐值哈希密码  
            byte[] hashedBytes = KeyDerivation.Pbkdf2(
            password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            string hashedValue = Convert.ToBase64String(hashedBytes);
            return hashedValue == hashed;
        }

        /// <summary>  
        /// 重写Equals方法以比较两个PasswordHash对象是否相等  
        /// </summary>  
        /// <param name="obj">另一个PasswordHash对象</param>  
        /// <returns>如果两个对象相等则返回true，否则返回false</returns>  
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (PasswordHash)obj;
            return HashValue == other.HashValue;
        }

        /// <summary>  
        /// 重写GetHashCode方法以确保两个相等的PasswordHash对象具有相同的哈希码  
        /// </summary>  
        /// <returns>哈希码</returns>  
        public override int GetHashCode()
        {
            return HashValue.GetHashCode();
        }
    }
}