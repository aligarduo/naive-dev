namespace NaiveDev.Infrastructure.Attributes
{
    /// <summary>
    /// Swagger显示位置属性
    /// </summary>
    /// <param name="position"></param>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SwaggerPositionAttribute(int position) : Attribute
    {
        /// <summary>
        /// 显示位置
        /// </summary>
        public int Position { get; } = position;
    }
}
