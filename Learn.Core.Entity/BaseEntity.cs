using Learn.Core.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Learn.Core.Entity
{
    public class BaseEntity
    {  /// <summary>
       /// 新增调用
       /// </summary>
        public virtual void Create()
        {
            //属性列表
            var properties = ReflectionHelper.GetProperties(this.GetType());
            //修改的键值
            var keys = GetDefaultForCreate();
            foreach (KeyValuePair<string, PropertyInfo> pair in properties)
            {
                //跳过自增列
                var identity_attributes = pair.Value.GetCustomAttributes(typeof(IdentityAttribute), true) as IdentityAttribute[];
                if (identity_attributes != null && identity_attributes.Length > 0)
                {
                    continue;
                }

                //设置主键id
                var attributes = pair.Value.GetCustomAttributes(typeof(PrimaryKeyAttribute), true) as Attribute[];
                if (attributes != null && attributes.Length > 0)
                {
                    if (pair.Value.PropertyType == typeof(string))
                    {
                        try
                        {
                            pair.Value.SetValue(this, Guid.NewGuid().ToString());
                        }
                        catch { }
                    }
                    else if (pair.Value.PropertyType == typeof(long) || pair.Value.PropertyType == typeof(long?))
                    {
                        if (pair.Value.GetValue(this) == null)
                        {
                            pair.Value.SetValue(this, SnowflakeHelper.NewId());
                        }
                        continue;
                    }
                }
                //设置字段默认值
                if (pair.Value.GetValue(this) == null)
                {
                    object vlue;
                    if (keys.TryGetValue(pair.Key.ToLower(), out vlue))
                    {
                        pair.Value.SetValue(this, vlue);
                    }
                }
            }
        }
        /// <summary>
        /// 新增调用
        /// </summary>
        public virtual void CreateApp()
        {
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue">主键值</param>
        public virtual void Modify(string keyValue)
        {
            Modify((object)keyValue);
        }
        /// <summary>
        /// 编辑调用，此方法适用于不需要为主键赋值的情况
        /// </summary>
        public virtual void Modify()
        {
            Modify((object)null);
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue">主键值</param>
        public virtual void Modify(object keyValue)
        {
            //属性列表
            var properties = ReflectionHelper.GetProperties(this.GetType());
            //修改的键值
            var keys = GetDefaultForModify();
            foreach (KeyValuePair<string, PropertyInfo> pair in properties)
            {
                if (keyValue != null)
                {
                    //设置主键id
                    var attributes = pair.Value.GetCustomAttributes(typeof(PrimaryKeyAttribute), true) as Attribute[];
                    if (attributes != null && attributes.Length > 0)
                    {
                        try
                        {
                            pair.Value.SetValue(this, keyValue);
                        }
                        catch
                        {
                        }
                    }
                }
                //设置字段默认值
                object vlue;
                if (keys.TryGetValue(pair.Key.ToLower(), out vlue))
                {
                    pair.Value.SetValue(this, vlue);
                }
            }
        }

        /// <summary>
        /// 删除调用
        /// </summary>
        /// <param name="keyValue">主键值</param>
        public virtual void Remove(string keyValue)
        {
        }

        /// <summary>
        /// 新增默认值
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetDefaultForCreate()
        {
            return new Dictionary<string, object>
                {
                    {"modifydate", DateTime.Now}
                };
            ////修复部分情况用户未登陆时提交数据异常
            //if (OperatorProvider.Provider == null || OperatorProvider.Provider.Current() == null)
            //{
            //    return new Dictionary<string, object>
            //    {
            //        {"createdate", DateTime.Now},
            //        {"modifydate", DateTime.Now}
            //    };
            //}
            //var user = OperatorProvider.Provider.Current();
            //var dict = new Dictionary<string, object>
            //{
            //    {"createdate", DateTime.Now},
            //    {"createuserid", user.UserId},
            //    {"createusername", user.UserName},
            //    {"modifydate", DateTime.Now},
            //    {"modifyuserid", user.UserId},
            //    {"modifyusername", user.UserName}
            //};
            //return dict;
        }

        /// <summary>
        /// 更新默认值
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetDefaultForModify()
        {
            return new Dictionary<string, object>
                {
                    {"modifydate", DateTime.Now}
                };
            ////修复部分情况用户未登陆时提交数据异常
            //if (OperatorProvider.Provider == null || OperatorProvider.Provider.Current() == null)
            //{
            //    return new Dictionary<string, object>
            //    {
            //        {"modifydate", DateTime.Now}
            //    };
            //}
            //var user = OperatorProvider.Provider.Current();
            //var dict = new Dictionary<string, object>
            //{
            //    {"modifydate", DateTime.Now},
            //    {"modifyuserid", user.UserId},
            //    {"modifyusername", user.UserName}
            //};
            //return dict;
        }
    }
}
