using System.Configuration;

namespace Learn.Core.Data.Repository
{
    /// <summary>
    /// 定义仓储模型工厂
    /// </summary>
    /// <typeparam name="T">动态实体类型</typeparam>
    public class RepositoryFactory<T> where T : class, new()
    {
        /// <summary>
        /// 定义仓储
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        public IRepository<T> BaseRepository(string connString)
        {
            return new Repository<T>(DbFactory.Base(connString, DatabaseType.SqlServer));
        }
        /// <summary>
        /// 定义仓储（基础库）
        /// </summary>
        /// <returns></returns>
        public IRepository<T> BaseRepository()
        {
            return new Repository<T>(DbFactory.Base());
        }


        #region 定义仓储 Dapper
        /// <summary>
        /// 定义仓储（基础库）Dapper
        /// </summary>
        /// <returns></returns>
        public IRepository<T> BaseRepositoryDapper()
        {
            return new Repository<T>(DbFactory.Base());
        }

        /// <summary>
        /// 定义仓储
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        public IRepository<T> BaseRepositoryDapper(string connString)
        {
            return new Repository<T>(DbFactory.Base(connString, DatabaseType.SqlServer));
        }
        #endregion
    }
}
