using System.Configuration;

namespace Learn.Data.Repository
{
    /// <summary>
    /// 定义仓储模型工厂
    /// </summary>
    public class RepositoryFactory
    {
        /// <summary>
        /// 定义仓储
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        public IRepository BaseRepository(string connString)
        {
            return new Repository(DbFactory.Base(connString, DatabaseType.SqlServer));
        }
        /// <summary>
        /// 定义仓储（基础库）
        /// </summary>
        /// <returns></returns>
        public IRepository BaseRepository()
        {
            return new Repository(DbFactory.Base());
        }


        #region 定义仓储 Dapper
        /// <summary>
        /// 定义仓储（基础库）Dapper
        /// </summary>
        /// <returns></returns>
        public IRepository BaseRepositoryDapper()
        {
            return new Repository(DbFactory.Base());
        }

        /// <summary>
        /// 定义仓储
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        public IRepository BaseRepositoryDapper(string connString)
        {
            return new Repository(DbFactory.Base(connString, DatabaseType.SqlServer));
        }
        #endregion
    }
}
