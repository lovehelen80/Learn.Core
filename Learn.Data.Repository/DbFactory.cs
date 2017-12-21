using Learn.Core.Util;
using System;
using Microsoft.Extensions.Configuration;

namespace Learn.Core.Data.Repository
{
    /// <summary>
    /// 数据工厂
    /// </summary>
    public class DbFactory
    {
        private static IConfiguration iConfiguration = null;
        static DbFactory()
        {
            iConfiguration = Config.AppSettings;
        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="connString">连接字符串</param>
        /// <param name="DbType">数据库类型</param>
        /// <returns></returns>
        public static IDatabase Base(string connString, DatabaseType DbType)
        {
            var connectionString = iConfiguration.GetConnectionString("Connection");
            switch (DbType)
            {
                case DatabaseType.MySql:
                    return new Learn.Core.Data.Dapper.MySqlDatabase(connectionString);
                    break;
                case DatabaseType.SqlServer:
                    return new Learn.Core.Data.Dapper.SqlDatabase(connectionString);
                    break;
                case DatabaseType.Oracle:
                    return null;
                    break;
                case DatabaseType.SQLite:
                    return null;
                    break;
                default:
                    throw new Exception("暂不支持您使用的数据库类型！");
                    break;
            }
        }
        /// <summary>
        /// 连接基础库
        /// </summary>
        /// <returns></returns>
        public static IDatabase Base()
        {
            var connectionString = iConfiguration.GetConnectionString("Connection");
            var providerName = iConfiguration.GetConnectionString("providerName");
            if (providerName.ToLower().Contains("mysql"))
            {
                return new Learn.Core.Data.Dapper.MySqlDatabase(connectionString);
            }
            else if (providerName.ToLower().Contains("sql"))
            {
                return new Learn.Core.Data.Dapper.SqlDatabase(connectionString);
            }
            throw new Exception("暂不支持您使用的数据库类型！");
        }
    }
}
