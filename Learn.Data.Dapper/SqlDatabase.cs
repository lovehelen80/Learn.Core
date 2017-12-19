using Dapper;
using Learn.Core.Util;
using Learn.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Learn.Data.Dapper
{
    /// <summary>
    /// 创建人：wengjianxun
    /// 日 期：2017.12.30
    /// 描 述：操作数据库
    /// </summary>
    public class SqlDatabase : IDatabase
    {
        #region 构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        public SqlDatabase(string connString)
        {
            DbHelper.DbType = DatabaseType.SqlServer;
            connectionString = connString;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取 数据库连接串
        /// </summary>
        public string connectionString { get; set; }
        protected DbConnection Connection
        {
            get
            {
                DbConnection dbconnection = new SqlConnection(connectionString);
                dbconnection.Open();
                return dbconnection;
            }
        }
        /// <summary>
        /// 事务对象
        /// </summary>
        public DbTransaction dbTransaction { get; set; }
        #endregion

        #region 事物提交
        /// <summary>
        /// 事务开始
        /// </summary>
        /// <returns></returns>
        public IDatabase BeginTrans()
        {
            DbConnection dbConnection = dbTransaction.Connection;
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
            dbTransaction = dbConnection.BeginTransaction();
            return this;
        }
        /// <summary>
        /// 提交当前操作的结果
        /// </summary>
        public int Commit()
        {
            try
            {
                if (dbTransaction != null)
                {
                    dbTransaction.Commit();
                    this.Close();
                }
                return 1;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.InnerException is SqlException)
                {
                    SqlException sqlEx = ex.InnerException.InnerException as SqlException;
                    string msg = ExceptionMessage.GetSqlExceptionMessage(sqlEx.Number);
                    throw DataAccessException.ThrowDataAccessException(sqlEx, msg);
                }
                throw;
            }
            finally
            {
                if (dbTransaction == null)
                {
                    this.Close();
                }
            }
        }
        /// <summary>
        /// 把当前操作回滚成未提交状态
        /// </summary>
        public void Rollback()
        {
            this.dbTransaction.Rollback();
            this.dbTransaction.Dispose();
            this.Close();
        }
        /// <summary>
        /// 关闭连接 内存回收
        /// </summary>
        public void Close()
        {
            DbConnection dbConnection = dbTransaction.Connection;
            if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
            {
                dbConnection.Close();
            }

        }
        #endregion

        #region 执行 SQL 语句
        private dynamic GetExpand(DbParameter[] dbParameter)
        {
            dynamic dobj = new System.Dynamic.ExpandoObject();

            var dic = (IDictionary<string, object>)dobj;
            foreach (var item in dbParameter)
            {
                dic[item.ParameterName] = item.Value;
            }
            return dobj;
        }


        public int ExecuteBySql(string strSql)
        {
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    return connection.Execute(strSql);
                }
            }
            else
            {
                dbTransaction.Connection.Execute(strSql, null, dbTransaction);
                return 0;

            }
        }



        public int ExecuteBySql(string strSql, params DbParameter[] dbParameter)
        {
            object param = GetExpand(dbParameter);
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    return connection.Execute(strSql, param);
                }
            }
            else
            {
                dbTransaction.Connection.Execute(strSql, param, dbTransaction);
                return 0;

            }
        }
        public int ExecuteByProc(string procName)
        {
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    return connection.Execute(procName);
                }
            }
            else
            {
                dbTransaction.Connection.Execute(procName, null, dbTransaction);
                return 0;

            }
        }
        public int ExecuteByProc(string procName, params DbParameter[] dbParameter)
        {
            object param = GetExpand(dbParameter);
            if (dbTransaction == null)
            {
                using (var connection = Connection)
                {
                    return connection.Execute(procName, param);
                }
            }
            else
            {
                dbTransaction.Connection.Execute(procName, param, dbTransaction);
                return 0;

            }
        }
        #endregion

        #region 对象实体 添加、修改、删除Dapper下未作实现，直接调用执行SQl方法
        /// <summary>
        /// 实体插入(dapper)不作实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert<T>(T entity) where T : class
        {
            return ExecuteBySql(DatabaseCommon.InsertSql<T>(entity).ToString(), DatabaseCommon.GetParameter<T>(entity));
        }
        /// <summary>
        /// 实体插入(dapper)不作实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Insert<T>(IEnumerable<T> entities) where T : class
        {
            if (dbTransaction == null)
            {
                BeginTrans();
                foreach (var item in entities)
                {
                    Insert<T>(item);
                }
                return Commit();
            }
            else
            {
                foreach (var item in entities)
                {
                    Insert<T>(item);
                }
                return 0;

            }
        }
        public int Delete<T>() where T : class
        {
            return ExecuteBySql(DatabaseCommon.DeleteSql(EntityAttribute.GetEntityTable<T>()).ToString());
        }
        public int Delete<T>(T entity) where T : class
        {
            return ExecuteBySql(DatabaseCommon.DeleteSql<T>(entity).ToString(), DatabaseCommon.GetParameter<T>(entity));
        }
        public int Delete<T>(IEnumerable<T> entities) where T : class
        {
            if (dbTransaction == null)
            {
                BeginTrans();
                foreach (var item in entities)
                {
                    Delete<T>(item);
                }
                return Commit();
            }
            else
            {
                foreach (var item in entities)
                {
                    Delete<T>(item);
                }
                return 0;
            }
        }
        public int Delete<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            bool isTrans = true;
            if (dbTransaction == null)
            {
                BeginTrans();
                isTrans = false;
            }
            IEnumerable<T> entities = dbTransaction.Connection.Query<T>(string.Format("select * from {0} where {1}  ", EntityAttribute.GetEntityTable<T>(), ExpressionHelper.DealExpress(condition)));
            Delete<T>(entities);
            if (!isTrans)
            {
                return Commit();
            }
            return 0;
        }

        public int Delete<T>(object keyValue) where T : class
        {
            T entity = dbTransaction.Connection.Query<T>(string.Format("select * from {0} where {1}=@primarykey", EntityAttribute.GetEntityTable<T>(), EntityAttribute.GetEntityKey<T>()), new { primarykey = keyValue }).FirstOrDefault();
            return Delete<T>(entity);
        }
        public int Delete<T>(object[] keyValue) where T : class
        {
            foreach (var item in keyValue)
            {
                Delete<T>(item);
            }
            return dbTransaction == null ? Commit() : 0;
        }
        public int Delete<T>(object propertyValue, string propertyName) where T : class
        {
            bool isTrans = true;
            if (dbTransaction == null)
            {
                BeginTrans();
                isTrans = false;
            }
            IEnumerable<T> entitys = dbTransaction.Connection.Query<T>(string.Format("select * from {0} where {1}=@propertyValue", propertyName), new { propertyValue = propertyValue });
            foreach (var entity in entitys)
            {
                Delete<T>(entity);
            }
            if (!isTrans)
            {
                return Commit();
            }
            return 0;
        }
        public int Update<T>(T entity) where T : class
        {
            return ExecuteBySql(DatabaseCommon.UpdateSql<T>(entity).ToString(), DatabaseCommon.GetParameter<T>(entity));
        }
        public int Update<T>(IEnumerable<T> entities) where T : class
        {
            if (dbTransaction == null)
            {
                BeginTrans();
                foreach (var item in entities)
                {
                    Update<T>(item);
                }
                return Commit();
            }
            else
            {
                foreach (var item in entities)
                {
                    Update<T>(item);
                }
                return 0;
            }
        }
        public int Update<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            bool isTrans = true;
            if (dbTransaction == null)
            {
                BeginTrans();
                isTrans = false;
            }
            IEnumerable<T> entities = dbTransaction.Connection.Query<T>(string.Format("select * from {0} where {1}  ", EntityAttribute.GetEntityTable<T>(), ExpressionHelper.DealExpress(condition)));
            Update<T>(entities);
            if (!isTrans)
            {
                return Commit();
            }
            return 0;
        }
        #endregion

        #region 对象实体 查询
        public T FindEntity<T>(object keyValue) where T : class
        {
            using (var dbConnection = Connection)
            {
                var sql = string.Format("select * from {0} where {1}=@primarykey", EntityAttribute.GetEntityTable<T>(), EntityAttribute.GetEntityPrimaryKey<T>());
                var param = new { primarykey = keyValue };
                var data = dbConnection.Query<T>(sql, param);
                return data.FirstOrDefault();
            }
        }
        public T FindEntity<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                var data = dbConnection.Query<T>(string.Format("select * from {0} where {1}  ", EntityAttribute.GetEntityTable<T>(), ExpressionHelper.DealExpress(condition)));
                return data.FirstOrDefault();
            }
        }
        /// <summary>
        /// 未实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> IQueryable<T>() where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                return (IQueryable<T>)dbConnection.Query<T>(string.Format("select * from {0} ", EntityAttribute.GetEntityTable<T>()));
            }
        }
        /// <summary>
        /// 未实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IQueryable<T> IQueryable<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                return (IQueryable<T>)dbConnection.Query<T>(string.Format("select * from {0} where {1}  ", EntityAttribute.GetEntityTable<T>(), ExpressionHelper.DealExpress(condition)));
            }
        }
        public IEnumerable<T> FindList<T>() where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                return dbConnection.Query<T>(string.Format("select * from {0}   ", EntityAttribute.GetEntityTable<T>())).ToList();
            }
        }
        public IEnumerable<T> FindList<T>(Func<T, object> keySelector) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                return dbConnection.Query<T>(string.Format("select * from {0}  ", EntityAttribute.GetEntityTable<T>())).OrderBy(keySelector).ToList();
            }
        }
        public IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                return dbConnection.Query<T>(string.Format("select * from {0} where {1}  ", EntityAttribute.GetEntityTable<T>(), ExpressionHelper.DealExpress(condition))).ToList();
            }
        }
        public IEnumerable<T> FindList<T>(string strSql) where T : class
        {
            return FindList<T>(strSql, null);
        }
        public IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class
        {
            using (var dbConnection = Connection)
            {
                return dbConnection.Query<T>(strSql, dbParameter);
            }
        }
        public IEnumerable<T> FindList<T>(string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                string[] _order = orderField.Split(',');
                var dataLinq = new StringBuilder(string.Format("select t.* from {0} as t  ORDER BY ", EntityAttribute.GetEntityTable<T>()));
                int fieldCount = 0;
                foreach (string item in _order)
                {
                    string _orderPart = item;
                    _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                    string[] _orderArry = _orderPart.Split(' ');
                    string _orderField = _orderArry[0];
                    bool sort = isAsc;
                    if (_orderArry.Length == 2)
                    {
                        isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                    }
                    var parameter = Expression.Parameter(typeof(T), "t");
                    var property = typeof(T).GetProperty(_orderField);
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    Expression<Func<T, object>> orderBy = t => propertyAccess;
                    if (fieldCount == _order.Length)
                    {
                        dataLinq.Append($" {parameter} {(isAsc ? "ASC" : "DESC")}");
                    }
                    else
                    {
                        dataLinq.Append($" {parameter} {(isAsc ? "ASC" : "DESC")},");
                    }
                    fieldCount++;
                }
                var dataQuery = dbConnection.Query<T>(dataLinq.ToString());
                total = dataQuery.Count();
                var data = dataQuery.Skip<T>(pageSize * (pageIndex - 1)).Take<T>(pageSize).AsQueryable();
                return data.ToList();
            }
        }
        public IEnumerable<T> FindList<T>(Expression<Func<T, bool>> condition, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class, new()
        {
            using (var dbConnection = Connection)
            {
                string[] _order = orderField.Split(',');
                var dataLinq = new SQLinq<T>().Where(condition);
                foreach (string item in _order)
                {

                    var parameter = Expression.Parameter(typeof(T), "t");
                    var property = typeof(T).GetProperty(item);
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    Expression<Func<T, object>> orderBy = t => propertyAccess;
                    dataLinq.OrderByExpressions.Add(new SQLinq<T>.OrderByExpression { Ascending = isAsc, Expression = orderBy });
                }
                var dataQuery = dbConnection.Query<T>(dataLinq);
                total = dataQuery.Count();
                var data = dataQuery.Skip<T>(pageSize * (pageIndex - 1)).Take<T>(pageSize).AsQueryable();
                return data.ToList();
            }
        }
        public IEnumerable<T> FindList<T>(string strSql, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class
        {
            return FindList<T>(strSql, null, orderField, isAsc, pageSize, pageIndex, out total);
        }
        public IEnumerable<T> FindList<T>(string strSql, DbParameter[] dbParameter, string orderField, bool isAsc, int pageSize, int pageIndex, out int total) where T : class
        {
            using (var dbConnection = Connection)
            {
                StringBuilder sb = new StringBuilder();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }
                int num = (pageIndex - 1) * pageSize;
                int num1 = (pageIndex) * pageSize;
                string OrderBy = "";

                if (!string.IsNullOrEmpty(orderField))
                {
                    if (orderField.ToUpper().IndexOf("ASC") + orderField.ToUpper().IndexOf("DESC") > 0)
                    {
                        OrderBy = "Order By " + orderField;
                    }
                    else
                    {
                        OrderBy = "Order By " + orderField + " " + (isAsc ? "ASC" : "DESC");
                    }
                }
                else
                {
                    OrderBy = "order by (select 0)";
                }
                sb.Append("Select * From (Select ROW_NUMBER() Over (" + OrderBy + ")");
                sb.Append(" As rowNum, * From (" + strSql + ") As T ) As N Where rowNum > " + num + " And rowNum <= " + num1 + "");
                total = Convert.ToInt32(new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, "Select Count(1) From (" + strSql + ") As t", dbParameter));
                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, sb.ToString(), dbParameter);
                return ConvertExtension.IDataReaderToList<T>(IDataReader);
            }
        }
        #endregion

        #region 数据源查询
        public DataTable FindTable(string strSql)
        {
            return FindTable(strSql, null);
        }
        public DataTable FindTable(string strSql, DbParameter[] dbParameter)
        {
            using (var dbConnection = Connection)
            {
                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, strSql, dbParameter);

                var table = new DataTable();
                table.Load(IDataReader);
                IDataReader.Close();
                return table;

                //return ConvertExtension.IDataReaderToDataTable(IDataReader);
            }
        }
        public DataTable FindTable(string strSql, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
        {
            return FindTable(strSql, null, orderField, isAsc, pageSize, pageIndex, out total);
        }
        public DataTable FindTable(string strSql, DbParameter[] dbParameter, string orderField, bool isAsc, int pageSize, int pageIndex, out int total)
        {
            using (var dbConnection = Connection)
            {
                StringBuilder sb = new StringBuilder();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }
                int num = (pageIndex - 1) * pageSize;
                int num1 = (pageIndex) * pageSize;
                string OrderBy = "";

                if (!string.IsNullOrEmpty(orderField))
                {
                    if (orderField.ToUpper().IndexOf("ASC") + orderField.ToUpper().IndexOf("DESC") > 0)
                    {
                        OrderBy = "Order By " + orderField;
                    }
                    else
                    {
                        OrderBy = "Order By " + orderField + " " + (isAsc ? "ASC" : "DESC");
                    }
                }
                else
                {
                    OrderBy = "order by (select 0)";
                }
                sb.Append("Select * From (Select ROW_NUMBER() Over (" + OrderBy + ")");
                sb.Append(" As rowNum, * From (" + strSql + ") As T ) As N Where rowNum > " + num + " And rowNum <= " + num1 + "");
                total = Convert.ToInt32(new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, "Select Count(1) From (" + strSql + ") As t", dbParameter));
                var IDataReader = new DbHelper(dbConnection).ExecuteReader(CommandType.Text, sb.ToString(), dbParameter);

                var resultTable = new DataTable();
                resultTable.Load(IDataReader);
                IDataReader.Close();

                //DataTable resultTable = ConvertExtension.IDataReaderToDataTable(IDataReader);
                resultTable.Columns.Remove("rowNum");
                return resultTable;
            }
        }
        public object FindObject(string strSql)
        {
            return FindObject(strSql, null);
        }
        public object FindObject(string strSql, DbParameter[] dbParameter)
        {
            using (var dbConnection = Connection)
            {
                return new DbHelper(dbConnection).ExecuteScalar(CommandType.Text, strSql, dbParameter);
            }
        }
        #endregion
    }
}
