using System.ComponentModel;
using System.Reflection;

namespace NaiveDev.Infrastructure.Tools
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string Description(this Enum @enum)
        {
            string value = @enum.ToString();
            FieldInfo? field = @enum.GetType().GetField(value);

            if (field is null)
            {
                return value;
            }

            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (objs is null || objs.Length is 0)
            {
                return value;
            }

            return ((DescriptionAttribute)objs[0]).Description;
        }
    }
}
