using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using NaiveDev.Infrastructure.Attributes;

namespace NaiveDev.Infrastructure.JsonConverters
{
    /// <summary>
    /// 数据脱敏JSON转换器工厂
    /// 用于处理包含<see cref="DataMaskAttribute"/>特性的类或对象的JSON序列化与反序列化
    /// </summary>
    public class DataMaskJsonConverter : JsonConverterFactory
    {
        /// <summary>
        /// 确定此转换器是否可以转换指定的类型
        /// </summary>
        /// <param name="typeToConvert">要检查的类型</param>
        /// <returns>如果类型包含带有<see cref="DataMaskAttribute"/>特性的字段或属性，则返回true；否则返回false</returns>
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert is null)
                return false;

            var memberInfos = typeToConvert.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Cast<MemberInfo>()
                    .Concat(typeToConvert.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            return memberInfos.Any(memberInfo => memberInfo.GetCustomAttributes(typeof(DataMaskAttribute), false).Length != 0);
        }

        /// <summary>
        /// 创建一个转换器实例，用于处理具有<see cref="DataMaskAttribute"/>特性的类型
        /// </summary>
        /// <param name="typeToConvert">要转换的类型</param>
        /// <param name="options">JSON序列化选项</param>
        /// <returns>返回一个<see cref="JsonConverter"/>实例，用于处理具有<see cref="DataMaskAttribute"/>特性的类型的序列化与反序列化</returns>
        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Activator.CreateInstance(
                typeof(DataMaskJsonConverterInner<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: [options],
                culture: null
            ) as JsonConverter;
        }
    }

    /// <summary>
    /// 自定义的JSON转换器，用于对T类型的对象进行脱敏处理
    /// </summary>
    /// <typeparam name="T">要转换和脱敏的对象的类型</typeparam>
    public class DataMaskJsonConverterInner<T> : JsonConverter<T>
    {
        /// <summary>
        /// 从JSON中反序列化对象（此方法本身不进行脱敏，仅作为JSON转换的入口）
        /// </summary>
        /// <param name="reader">包含要反序列化的JSON的Utf8JsonReader</param>
        /// <param name="typeToConvert">要转换到的类型</param>
        /// <param name="options">JSON序列化选项</param>
        /// <returns>反序列化后的对象</returns>
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(ref reader, options);
        }

        /// <summary>
        /// 将对象序列化为JSON，并进行脱敏处理
        /// </summary>
        /// <param name="writer">用于写入JSON的Utf8JsonWriter</param>
        /// <param name="value">要序列化的对象</param>
        /// <param name="options">JSON序列化选项</param>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // 使用字典来存储属性名和脱敏后的值
            Dictionary<string, object?> propertyValues = [];

            // 获取T类型的所有可读公共实例属性
            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.GetMethod != null);
            foreach (var property in properties)
            {
                // 获取属性值
                object? valueToSerialize = property.GetValue(value);
                // 检查属性上是否有DataMaskAttribute特性
                DataMaskAttribute? dataMaskAttribute = property.GetCustomAttribute<DataMaskAttribute>();

                // 如果属性上有DataMaskAttribute特性，则进行脱敏处理
                if (dataMaskAttribute is not null)
                {
                    string? maskedValue = string.Empty;

                    // 如果有自定义脱敏规则，则使用自定义规则
                    if (!string.IsNullOrEmpty(dataMaskAttribute.CustomRule))
                    {
                        maskedValue = CustomRuleMask(dataMaskAttribute.CustomRule, valueToSerialize?.ToString());
                    }
                    else
                    {
                        // 否则根据MaskMethod进行不同的脱敏处理
                        switch (dataMaskAttribute.MaskMethod)
                        {
                            case MaskMethod.Name:
                                maskedValue = MaskName(valueToSerialize?.ToString());
                                break;
                            case MaskMethod.Phone:
                                maskedValue = MaskPhone(valueToSerialize?.ToString());
                                break;
                            case MaskMethod.Mail:
                                maskedValue = MaskMail(valueToSerialize?.ToString());
                                break;
                            default:
                                break;
                        }
                    }

                    // 将脱敏后的值替换为原值
                    valueToSerialize = maskedValue;
                }

                // 将属性名和脱敏后的值添加到字典中
                propertyValues.Add(property.Name, valueToSerialize);
            }

            // 序列化字典到writer中
            JsonSerializer.Serialize(writer, propertyValues, options);
        }

        /// <summary>
        /// 脱敏名字（将除首尾字符外的字符替换为*）
        /// </summary>
        /// <param name="name">要脱敏的名字</param>
        /// <returns>脱敏后的名字</returns>
        private static string? MaskName(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            if (name.Length == 1)
                return name[0] + "*";

            return name[0] + new string('*', name.Length - 2) + name[^1];
        }

        /// <summary>
        /// 脱敏手机号码（将中间字符替换为*）
        /// </summary>
        /// <param name="phone">要脱敏的手机号码</param>
        /// <returns>脱敏后的手机号码</returns>
        private static string? MaskPhone(string? phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 4)
                return "*";

            if (phone.Length >= 7)
                return phone[..3] + "****" + phone[^4..];

            return phone[0] + new string('*', phone.Length - 2) + phone[^1];
        }

        /// <summary>
        /// 脱敏邮箱（将邮箱用户名中的字符替换为*，保留@及之后的部分）
        /// </summary>
        /// <param name="email">要脱敏的邮箱地址</param>
        /// <returns>脱敏后的邮箱地址</returns>
        private static string? MaskMail(string? email)
        {
            if (string.IsNullOrEmpty(email))
                return "";

            int atIndex = email.IndexOf('@');

            if (atIndex <= 0)
            {  
                if (atIndex == 0)
                    return "@*";

                return "*";
            }

            if (atIndex == 1)
            {
                return string.Concat("*", email.AsSpan(atIndex));
            }

            return email[0] + new string('*', atIndex - 2) + email[atIndex..];
        }

        /// <summary>
        /// 使用自定义的正则表达式进行脱敏（将匹配到的字符替换为*）
        /// </summary>
        /// <param name="customRule">自定义的正则表达式</param>
        /// <param name="value">要脱敏的值</param>
        /// <returns>脱敏后的值</returns>
        public static string? CustomRuleMask(string customRule, string? value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return new Regex(customRule).Replace(value, m => new string('*', m.Length));
        }
    }
}
