using NaiveDev.Domain.ValueObjects;
using NaiveDev.Infrastructure.Entities;
using NaiveDev.Infrastructure.Tools;

using SqlSugar;

namespace NaiveDev.Domain.Entities
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    [Tenant("OSS")]
    [SugarTable("user", "用户信息表")]
    public class User : EntityBase
    {
        /// <summary>
        /// 公共无参构造函数，配合ORM框架使用
        /// </summary>
        public User() { }

        /// <summary>
        /// 私有构造函数，不能被外部直接调用
        /// </summary>
        private User(UserAccount account, UserName username, PasswordHash password, string salt) : base()
        {
            Id = new SnowflakeIdHelper(1, 1).NextId();
            Account = account.Value;
            Name = username.Value;
            Password = password.HashValue;
            Salt = salt;
        }

        /// <summary>
        /// 公共静态工厂方法，用于创建有效的用户实体
        /// </summary>
        public static User Create(string? username, string? password)
        {
            string salt = PasswordSalt.Create().SaltValue;

            return new User(UserAccount.Create(), UserName.Create(username), PasswordHash.Create(password, salt), salt);
        }

        /// <summary>
        /// 账号
        /// </summary>
        [SugarColumn(ColumnName = "account", IsNullable = false, ColumnDescription = "账号", Length = 16)]
        public string Account { get; init; }

        /// <summary>
        /// 用户名
        /// </summary>
        [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "用户名", Length = 64)]
        public string Name { get; init; }

        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(ColumnName = "password", IsNullable = false, ColumnDescription = "密码", Length = 256)]
        public string Password { get; init; }

        /// <summary>
        /// 盐
        /// </summary>
        [SugarColumn(ColumnName = "salt", IsNullable = false, ColumnDescription = "盐", Length = 64)]
        public string Salt { get; init; }
    }
}