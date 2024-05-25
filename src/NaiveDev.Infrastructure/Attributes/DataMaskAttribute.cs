namespace NaiveDev.Infrastructure.Attributes
{
    /// <summary>
    /// 数据脱敏特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DataMaskAttribute : Attribute
    {
        /// <summary>
        /// 自定义规则：正则表达式
        /// </summary>
        public string? CustomRule { get; set; }

        /// <summary>
        /// 数据脱敏方式
        /// </summary>
        public MaskMethod? MaskMethod { get; set; }

        /// <summary>
        /// 数据脱敏特性
        /// </summary>
        /// <param name="CustomRule">自定义规则</param>
        public DataMaskAttribute(string CustomRule) => this.CustomRule = CustomRule;

        /// <summary>
        /// 数据脱敏特性
        /// </summary>
        /// <param name="MaskMethod">脱敏方式</param>
        public DataMaskAttribute(MaskMethod MaskMethod) => this.MaskMethod = MaskMethod;
    }

    /// <summary>
    /// 脱敏方式
    /// </summary>
    public enum MaskMethod
    {
        /// <summary>
        /// 名字
        /// </summary>
        Name,
        /// <summary>
        /// 手机
        /// </summary>
        Phone,
        /// <summary>
        /// 电子邮箱
        /// </summary>
        Mail,
    }
}
