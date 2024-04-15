namespace NaiveDev.Infrastructure.Entities
{
    /// <summary>
    /// 值对象基类
    /// </summary>
    public abstract class ValueObjectBase
    {
        /// <summary>
        /// 比较两个ValueObject实例是否相等
        /// </summary>
        /// <param name="left">左侧的ValueObject实例</param>
        /// <param name="right">右侧的ValueObject实例</param>
        /// <returns>如果两个实例相等则返回true；否则返回false</returns>
        protected static bool Equals(ValueObjectBase left, ValueObjectBase right)
        {
            // 如果左侧实例为null而右侧实例不为null，或者左侧实例不为null而右侧实例为null，则返回false  
            if (left is null || right is null)
            {
                return false;
            }

            // 如果左侧实例和右侧实例引用同一个对象，则返回true  
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            // 获取左侧实例的所有可读属性及其值  
            var leftProperties = left.GetType().GetProperties().Where(prop => prop.CanRead).Select(prop => prop.GetValue(left));
            // 获取右侧实例的所有可读属性及其值  
            var rightProperties = right.GetType().GetProperties().Where(prop => prop.CanRead).Select(prop => prop.GetValue(right));

            // 使用SequenceEqual方法比较两个属性序列是否相等  
            return leftProperties.SequenceEqual(rightProperties);
        }

        /// <summary>
        /// 比较当前实例与另一个ValueObject实例是否相等
        /// </summary>  
        /// <param name="obj">要比较的另一个ValueObject实例</param>
        /// <returns>如果相等则返回true；否则返回false</returns>
        protected bool Equals(ValueObjectBase obj)
        {
            return Equals(this, obj);
        }

        /// <summary>
        /// 重写Equals方法，用于比较当前实例与任意对象是否相等
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>如果obj为null或者不是当前实例的相同类型，则返回false
        /// 否则调用Equals(ValueObject obj)方法进行比较</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            if (obj is not ValueObjectBase valueObjectBase)
            {
                return false;
            }

            return Equals(valueObjectBase);
        }

        /// <summary>
        /// 重写GetHashCode方法，返回当前实例的哈希码
        /// </summary>
        /// <returns>当前实例的哈希码</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                // 哈希码初始值
                var hashCode = 19;
                // 获取所有可读属性
                var properties = this.GetType().GetProperties().Where(prop => prop.CanRead);
                foreach (var property in properties)
                {
                    // 获取属性的值
                    var value = property.GetValue(this);
                    if (value is not null)
                    {
                        // 根据属性值更新哈希码
                        hashCode = hashCode * 31 + value.GetHashCode();
                    }
                }

                // 返回最终的哈希码
                return hashCode;
            }
        }
    }
}
