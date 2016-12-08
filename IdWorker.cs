using System;

namespace ToolGood.ReadyGo
{
    /// <summary>
    /// 全局ID生成器，Twitter雪花算法（简化版），ID递增。
    /// 支持32台机器，从0开始，
    /// 每台每秒可生成26.8万个ID，
    /// 可使用到36856年
    /// </summary>
    public class IdWorker
    {
        //机器标识位数
        const int WorkerIdBits = 5;
        //序列号识位数
        const int SequenceBits = 18;
        //机器ID最大值
        const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        //序列号ID最大值
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);
        //机器ID偏左移12位
        private const int WorkerIdShift = SequenceBits;
        //时间毫秒左移22位
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits;

        private long _sequence = 0L;
        private long _lastTimestamp = -1L;

        public long WorkerId { get; protected set; }
        public long Sequence { get { return _sequence; } internal set { _sequence = value; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workerId">机器标识</param>
        /// <param name="sequence">序列,小于0则随机</param>
        public IdWorker(long workerId, long sequence = -1L)
        {
            // 如果超出范围就抛出异常
            if (workerId > MaxWorkerId || workerId < 0) {
                throw new ArgumentException(string.Format("worker Id 必须大于0，且不能大于MaxWorkerId： {0}", MaxWorkerId));
            }
            WorkerId = workerId;
            if (sequence < 0L) {
                _sequence = new Random().Next((int)SequenceMask);
            } else {
                _sequence = sequence;
            }
        }

        readonly object _lock = new Object();
        public virtual long NextId()
        {
            lock (_lock) {
                var timestamp = TimeGen();
                if (timestamp < _lastTimestamp) {
                    throw new Exception(string.Format("时间戳必须大于上一次生成ID的时间戳.  拒绝为{0}毫秒生成id", _lastTimestamp - timestamp));
                }

                //如果上次生成时间和当前时间相同,在同一毫秒内
                if (_lastTimestamp == timestamp) {
                    //sequence自增，和sequenceMask相与一下，去掉高位
                    _sequence = (_sequence + 1) & SequenceMask;
                    //判断是否溢出,也就是每毫秒内超过1024，当为1024时，与sequenceMask相与，sequence就等于0
                    if (_sequence == 0) {
                        //等待到下一毫秒
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                } else {
                    //如果和上次生成时间不同,重置sequence，就是下一毫秒开始，sequence计数重新从0开始累加,
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;
                return (timestamp << TimestampLeftShift) | (WorkerId << WorkerIdShift) | _sequence;
            }
        }

        // 防止产生的时间比之前的时间还要小（由于NTP回拨等问题）,保持增量的趋势.
        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp) {
                timestamp = TimeGen();
            }
            return timestamp;
        }
        private static readonly DateTime Twepoch = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // 获取当前的时间戳
        protected virtual long TimeGen()
        {
            return (long)(DateTime.UtcNow - Twepoch).TotalSeconds;
        }
    }
}