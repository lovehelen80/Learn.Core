using Snowflake;

namespace Learn.Core.Util
{
    /// <summary>
    /// 雪花算法帮助类
    /// </summary>
    public class SnowflakeHelper
    {
        private static IdWorker worker = new IdWorker(1, 1);

        /// <summary>
        /// 获取全局ID
        /// </summary>
        /// <returns></returns>
        public static long NewId()
        {
            return worker.NextId();
        }
    }
}
