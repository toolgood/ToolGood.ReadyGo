using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace ToolGood.ReadyGo3
{
    /// <summary>
    /// 全局ID,Twitter 雪花算法简单实现 修改
    /// 支持32台机器
    /// 每台每秒可产生30万个序号
    /// 40位时间截长度,可使用2万年
    /// </summary>
    public class IdWorker
    {
        //机器标识位数
        private const int WorkerIdBits = 5;
        //序列号识位数
        private const int SequenceBits = 18;
        //机器ID最大值
        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        //序列号ID最大值
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);
        //机器ID偏左移12位
        private const int WorkerIdShift = SequenceBits;
        //时间秒左移22位
        private const int TimestampLeftShift = SequenceBits + WorkerIdBits;

        private long _sequence = 0L;
        private long _lastTimestamp = -1L;
        /// <summary>
        /// 机器标识
        /// </summary>
        public long WorkerId { get; private set; }
 
        /// <summary>
        /// IdWorker
        /// </summary>
        /// <param name="workerId">机器标识</param>
        /// <param name="sequence">序列ID</param>
        public IdWorker(long workerId = 0, long sequence = 0L)
        {
            // 如果超出范围就抛出异常
            if (workerId > MaxWorkerId || workerId < 0) {
                throw new ArgumentException($"worker Id 必须大于0，且不能大于MaxWorkerId： {MaxWorkerId}");
            }

            //先检验再赋值
            WorkerId = workerId;
            _sequence = sequence;
        }

        readonly object _lock = new Object();
        /// <summary>
        /// 获取下一个ID
        /// </summary>
        /// <returns></returns>
        public virtual long NextId()
        {
            ////雪花算法(Snowflake)C#版本 压测Id重复严重
            ////https://www.cnblogs.com/yushuo/p/9406906.html
            lock (_lock) {
                var timestamp = TimeGen();
                if (timestamp < _lastTimestamp) {
                    throw new Exception($"时间戳必须大于上一次生成ID的时间戳.  拒绝为{_lastTimestamp - timestamp}毫秒生成id");
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
        private long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp) {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        private static readonly long Twepoch = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        // 获取当前的时间戳
        private long TimeGen()
        {
            //近似取秒的时间戳，实际比1秒更短
            //比以下代码效率更高
            //private static readonly DateTime Twepoch1 = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //return (long)(DateTime.UtcNow - Twepoch1).TotalSeconds
            return (DateTime.UtcNow.Ticks - Twepoch) >> 23;
        }



        private static DateTime dt1970 = new DateTime(1970, 1, 1);
        private static Random rnd = new Random();
        private static readonly int __staticMachine = ((0x00ffffff & Environment.MachineName.GetHashCode()) +AppDomain.CurrentDomain.Id) & 0x00ffffff;
        private static readonly int __staticPid = Process.GetCurrentProcess().Id;
        private static int __staticIncrement = rnd.Next();

        /// <summary>
        /// 生成类似Mongodb的ObjectId有序、不重复Guid
        /// https://www.cnblogs.com/yushuo/p/9406906.html 回复2楼
        /// </summary>
        /// <returns></returns>
        public static Guid NewMongodbId()
        {
            var now = DateTime.Now;
            var uninxtime = (int)now.Subtract(dt1970).TotalSeconds;
            int increment = Interlocked.Increment(ref __staticIncrement) & 0x00ffffff;
            var rand = rnd.Next(0, int.MaxValue);
            var guid = $"{uninxtime.ToString("x8").PadLeft(8, '0')}{__staticMachine.ToString("x8").PadLeft(8, '0').Substring(2, 6)}{__staticPid.ToString("x8").PadLeft(8, '0').Substring(6, 2)}{increment.ToString("x8").PadLeft(8, '0')}{rand.ToString("x8").PadLeft(8, '0')}";
            return Guid.Parse(guid);
        }
    }

}
