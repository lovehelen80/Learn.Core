using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Linq;

namespace Learn.Core.Util
{
    /// <summary>
    /// 获取实体类Attribute自定义属性
    /// </summary>
    public class EntityAttribute
    {
        /// <summary>
        ///  获取实体对象Key
        /// </summary>
        /// <returns></returns>
        public static string GetEntityKey<T>()
        {
            Type type = typeof(T);
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                foreach (System.Attribute attr in prop.GetCustomAttributes(true))
                {
                    KeyAttribute keyattribute = attr as KeyAttribute;
                    if (keyattribute != null)
                    {
                        return prop.Name;
                    }
                }
            }
            return null;
        }
        public static string GetEntityPrimaryKey<T>()
        {
            Type type = typeof(T);
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                foreach (System.Attribute attr in prop.GetCustomAttributes(true))
                {
                    var keyattribute = attr as PrimaryKeyAttribute;
                    if (keyattribute != null)
                    {
                        return prop.Name;
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///  获取实体对象表名
        /// </summary>
        /// <returns></returns>
        public static string GetEntityTable<T>()
        {
            Type objTye = typeof(T);
            string entityName = "";
            var tableAttribute = objTye.GetCustomAttributes(true).OfType<TableAttribute>();
            var descriptionAttributes = tableAttribute as TableAttribute[] ?? tableAttribute.ToArray();
            if (descriptionAttributes.Any())
                entityName = descriptionAttributes.ToList()[0].Name;
            else
            {
                entityName = objTye.Name;
            }
            return entityName;
        }
        /// <summary>
        /// 获取实体对象表名
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static string GetTable(Type entityType)
        {
            try
            {
                var table = entityType.GetCustomAttributes(typeof(TableAttribute), true);
                return ((TableAttribute)table[0]).Name;
            }
            catch
            {
                throw new Exception(ExceptionText.NoTableName);
            }

        }
    }
}
