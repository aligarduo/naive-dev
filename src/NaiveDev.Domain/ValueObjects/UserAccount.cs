using NaiveDev.Infrastructure.Entities;

namespace NaiveDev.Domain.ValueObjects
{
    /// <summary>
    /// 用户账号值对象
    /// </summary>
    public class UserAccount : ValueObjectBase
    {
        /// <summary>  
        /// 账号的值  
        /// </summary>  
        public string Value { get; }

        private static Queue<string> accountBucket = new();
        private static Random random = new();

        /// <summary>  
        /// 私有构造函数，确保对象的不变性  
        /// </summary>  
        private UserAccount(string value)
        {
            Value = value;
        }

        /// <summary>
        /// 静态工厂方法，用于创建 Account 实例
        /// </summary>
        /// <returns></returns>
        public static UserAccount Create()
        {
            if (accountBucket.Count == 0)
            {
                RefillBucket();
            }

            string uniqueAccount = accountBucket.Dequeue();
            return new UserAccount(uniqueAccount);
        }

        private static void RefillBucket()
        {
            for (int i = 0; i < 20; i++)
            {
                int uniqueAccount = random.Next(1000000000, 2000000000);
                accountBucket.Enqueue(uniqueAccount.ToString());
            }
        }
    }
}