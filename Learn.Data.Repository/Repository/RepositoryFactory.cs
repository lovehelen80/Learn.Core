using System.Configuration;

namespace Learn.Data.Repository
{
    /// <summary>
    /// 定义仓储模型工厂
    /// </summary>
    public class RepositoryFactory
    {
        #region 定义仓储 Dapper
        private static readonly string BaseDb = ConfigurationManager.ConnectionStrings["BaseDb"].ConnectionString;
        /// <summary>
        /// 定义仓储（基础库）Dapper
        /// </summary>
        /// <returns></returns>
        public IRepository BaseRepositoryDapper()
        {
            return BaseRepositoryDapper(BaseDb);
        }

        /// <summary>
        /// 定义仓储
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        public IRepository BaseRepositoryDapper(string connString)
        {
            IDatabase db = new Learn.Data.Dapper.SqlDatabase(connString);

            Repository repository = new Repository(db);

            return repository;
        }
        #endregion
    }
}
