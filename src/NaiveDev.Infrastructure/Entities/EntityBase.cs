using SqlSugar;

namespace NaiveDev.Infrastructure.Entities
{
    /// <summary>
    /// 领域实体基类
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// 构造函数，初始化创建于和修改于时间
        /// </summary>
        public EntityBase()
        {
            CreatedAt = DateTime.Now;
            ModifyedAt = default;
        }

        /// <summary>
        /// 更改数据后记录更改信息
        /// </summary>
        protected virtual void UpdateRecord()
        {
            ModifyedAt = DateTime.Now;
        }

        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "主键")]
        public virtual long Id { get; set; }

        /// <summary>
        /// 创建于
        /// </summary>
        [SugarColumn(ColumnName = "created_at", IsNullable = false, ColumnDescription = "创建于")]
        public virtual DateTime CreatedAt { get; set; }

        /// <summary>
        /// 修改于
        /// </summary>
        [SugarColumn(ColumnName = "modifyed_at", IsNullable = false, ColumnDescription = "修改于")]
        public virtual DateTime ModifyedAt { get; set; }
    }
}