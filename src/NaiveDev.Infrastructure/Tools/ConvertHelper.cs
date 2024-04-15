using Newtonsoft.Json;

namespace NaiveDev.Infrastructure.Tools
{
    /// <summary>
    /// 转换帮助类
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// 将对象转换为整数
        /// </summary>
        /// <param name="obj">要转换为整数的对象</param>
        /// <returns>转换后的整数值，如果转换失败则返回 0</returns>
        public static int ToInt(this object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            if (obj != null && obj != DBNull.Value && int.TryParse(obj.ToString(), out int result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// 将对象转换为单精度浮点数
        /// </summary>
        /// <param name="obj">要转换为单精度浮点数的对象</param>
        /// <returns>转换后的单精度浮点数值，如果转换失败则返回 0</returns>
        public static float ToSingle(this object obj)
        {
            if (obj != null && obj != DBNull.Value && Single.TryParse(obj.ToString(), out Single result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// 将对象转换为双精度浮点数
        /// </summary>
        /// <param name="obj">要转换为双精度浮点数的对象</param>
        /// <returns>转换后的双精度浮点数值，如果转换失败则返回 0</returns>
        public static double ToDouble(this object obj)
        {
            if (obj != null && obj != DBNull.Value && double.TryParse(obj.ToString(), out double result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// 将对象转换为十进制数，如果转换失败则返回0
        /// </summary>
        /// <param name="obj">要转换为十进制数的对象</param>
        /// <returns>转换后的十进制数值，如果转换失败则返回 0</returns>
        public static decimal ToDecimal(this object obj)
        {
            if (obj != null && obj != DBNull.Value && decimal.TryParse(obj.ToString(), out decimal result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// 将对象转换为日期时间
        /// </summary>
        /// <param name="obj">要转换为日期时间的对象</param>
        /// <returns>转换后的日期时间值，如果转换失败则返回 DateTime.MinValue</returns>
        public static DateTime ToDate(this object obj)
        {
            if (obj != null && obj != DBNull.Value && DateTime.TryParse(obj.ToString(), out DateTime result))
            {
                return result;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// 将对象转换为布尔值
        /// </summary>
        /// <param name="obj">要转换为布尔值的对象</param>
        /// <returns>转换后的布尔值，如果转换失败则返回 false</returns>
        public static bool ToBool(this object obj)
        {
            if (obj != null && obj != DBNull.Value && bool.TryParse(obj.ToString(), out bool result))
            {
                return result;
            }

            return false;
        }

        /// <summary>
        /// 将对象转换为JSON字符串
        /// </summary>
        /// <param name="obj">要转换为JSON字符串的对象</param>
        /// <returns>转换后的JSON字符串</returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将JSON字符串转换为指定类型的对象
        /// </summary>
        /// <typeparam name="T">要转换的目标类型</typeparam>
        /// <param name="json">包含要反序列化的JSON数据的字符串</param>
        /// <returns>反序列化后的指定类型的对象，如果转换失败则返回 null</returns>
        public static T To<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json) ?? throw new ArgumentNullException(nameof(json));
        }

        /// <summary>
        /// 将对象转换为JSON字符串，然后再将JSON字符串转换为指定类型的对象
        /// </summary>
        /// <typeparam name="T">要转换的目标类型</typeparam>
        /// <param name="obj">要序列化为JSON字符串并反序列化的对象</param>
        /// <returns>反序列化后的指定类型的对象，如果转换失败则返回 null</returns>
        public static T To<T>(this object obj)
        {
            return JsonConvert.DeserializeObject<T>(obj.ToJson()) ?? throw new ArgumentNullException(nameof(obj));
        }
    }
}
