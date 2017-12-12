using System.Configuration;

namespace Learn.Data.Repository
{
    /// <summary>
    /// 定义仓储模型工厂
    /// </summary>
    /// <typeparam name="T">动态实体类型</typeparam>
    public class RepositoryFactory<T> where T : class, new()
    {
        #region 定义仓储 Dapper
        private static readonly string BaseDb = ConfigurationManager.ConnectionStrings["BaseDb"].ConnectionString;
        /// <summary>
        /// 定义仓储（基础库）Dapper
        /// </summary>
        /// <returns></returns>
        public IRepository<T> BaseRepositoryDapper()
        {
            return BaseRepositoryDapper(BaseDb);
        }

        /// <summary>
        /// 定义仓储
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <returns></returns>
        public IRepository<T> BaseRepositoryDapper(string connString)
        {
            IDatabase db = new Learn.Data.Dapper.SqlDatabase(connString);

            Repository<T> repository = new Repository<T>(db);

            return repository;
        }
        #endregion
    }
}
