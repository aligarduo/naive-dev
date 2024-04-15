namespace NaiveDev.Infrastructure.Tools
{
    /// <summary>
    /// 雪花Id帮助类
    /// </summary>
    public class SnowflakeIdHelper
    {
        /// <summary>  
        /// 起始的时间戳（可以根据业务进行设定）  
        /// </summary>  
        private static readonly long Twepoch = 1288834974657L;

        /// <summary>  
        /// 机器id所占的位数  
        /// </summary>  
        private const int WorkerIdBits = 5;

        /// <summary>  
        /// 数据标识id所占的位数  
        /// </summary>  
        private const int DatacenterIdBits = 5;

        /// <summary>  
        /// 最大支持的机器id数量  
        /// </summary>  
        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);

        /// <summary>  
        /// 最大支持的数据中心id数量  
        /// </summary>  
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

        /// <summary>  
        /// 序列在id中占的位数  
        /// </summary>  
        private const int SequenceBits = 12;

        /// <summary>  
        /// 机器ID左移位数  
        /// </summary>  
        private const int WorkerIdShift = SequenceBits;

        /// <summary>  
        /// 数据中心ID左移位数  
        /// </summary>  
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;

        /// <summary>  
        /// 时间截左移位数  
        /// </summary>  
        private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

        /// <summary>  
        /// 序列掩码  
        /// </summary>  
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        /// <summary>  
        /// 上一次生成ID的时间截  
        /// </summary>  
        private long lastTimestamp = -1L;

        /// <summary>  
        /// 序列（0-4095）  
        /// </summary>  
        private long sequence = 0L;

        /// <summary>  
        /// 工作机器ID  
        /// </summary>  
        private long workerId;

        /// <summary>  
        /// 数据中心ID  
        /// </summary>  
        private long datacenterId;

        /// <summary>    
        /// 初始化雪花算法ID生成器  
        /// </summary>    
        /// <param name="workerId">工作机器ID，标识唯一的工作机器节点，应在指定的范围内</param>    
        /// <param name="datacenterId">数据中心ID，标识唯一的数据中心节点，应在指定的范围内</param>    
        /// <exception cref="ArgumentOutOfRangeException">当工作机器ID或数据中心ID超出指定范围时，抛出此异常</exception>  
        public SnowflakeIdHelper(long workerId, long datacenterId)
        {
            // 检查工作机器ID是否超出最大允许范围或小于0  
            if (workerId > MaxWorkerId || workerId < 0)
            {
                // 抛出参数超出范围异常，并提示用户工作机器ID的合法范围  
                throw new ArgumentOutOfRangeException(nameof(workerId), $"工作机器ID不能大于{MaxWorkerId}或小于0");
            }
            // 检查数据中心ID是否超出最大允许范围或小于0  
            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                // 抛出参数超出范围异常，并提示用户数据中心ID的合法范围  
                throw new ArgumentOutOfRangeException(nameof(datacenterId), $"数据中心ID不能大于{MaxDatacenterId}或小于0");
            }
            // 初始化当前实例的工作机器ID  
            this.workerId = workerId;
            // 初始化当前实例的数据中心ID  
            this.datacenterId = datacenterId;
        }

        /// <summary>    
        /// 生成下一个唯一的ID。  
        /// </summary>    
        /// <returns>生成的唯一ID</returns>    
        /// <exception cref="Exception">当系统时钟回退时，抛出异常。</exception>    
        public long NextId()
        {
            // 使用lock关键字进行同步，确保在同一时刻只有一个线程可以访问NextId方法内部的代码  
            lock (this)
            {
                // 生成当前时间的时间戳  
                long timestamp = TimeGen();

                // 如果当前时间戳小于上一次记录的时间戳，说明系统时钟发生了回退  
                if (timestamp < lastTimestamp)
                {
                    // 抛出异常，并说明时钟回退了多久（以毫秒为单位）  
                    throw new Exception(string.Format("系统时钟回退。拒绝生成ID，因为时钟回退了 {0} 毫秒", lastTimestamp - timestamp));
                }

                // 如果当前时间戳与上一次记录的时间戳相同  
                if (lastTimestamp == timestamp)
                {
                    // 序列号自增，并与SequenceMask进行按位与操作，以确保序列号的范围在0到SequenceMask之间  
                    sequence = (sequence + 1) & SequenceMask;

                    // 如果序列号自增后变为0，说明在同一毫秒内已经生成了足够的ID，需要等待下一个毫秒  
                    if (sequence == 0)
                    {
                        // 调用TilNextMillis方法，等待直到下一个毫秒并获取新的时间戳  
                        timestamp = TilNextMillis(lastTimestamp);
                    }
                }
                // 如果当前时间戳与上一次记录的时间戳不同（即跨毫秒了）  
                else
                {
                    // 重置序列号为0，因为新毫秒的开始  
                    sequence = 0L;
                }

                // 更新最后记录的时间戳  
                lastTimestamp = timestamp;

                // 计算并返回最终的ID  
                // ID由四部分组成：时间戳左移一定位数后的值、数据中心ID左移一定位数后的值、工作机器ID左移一定位数后的值以及序列号  
                // 通过位移和按位或操作将这些部分组合起来，形成一个唯一的ID  
                return ((timestamp - Twepoch) << TimestampLeftShift) | (datacenterId << DatacenterIdShift) | (workerId << WorkerIdShift) | sequence;
            }
        }

        /// <summary>  
        /// 获取下一个时间戳（毫秒为单位），确保新的时间戳大于上一个时间戳。  
        /// </summary>  
        /// <param name="lastTimestamp">上一个时间戳（毫秒为单位）</param>  
        /// <returns>下一个时间戳（毫秒为单位）</returns>  
        private long TilNextMillis(long lastTimestamp)
        {
            // 生成当前时间戳  
            long timestamp = TimeGen();
            // 如果生成的时间戳不大于上一个时间戳，则继续生成，直到大于上一个时间戳为止  
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            // 返回新的时间戳  
            return timestamp;
        }

        /// <summary>  
        /// 生成当前时间的时间戳（毫秒为单位），基于UTC时间。  
        /// </summary>  
        /// <returns>当前时间的时间戳（毫秒为单位）</returns>  
        private long TimeGen()
        {
            // 获取当前时间与1970年1月1日00:00:00 UTC之间的时间差（毫秒为单位）  
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}
