using System.ComponentModel;

namespace Learn.Core.Util
{
    public static class ExceptionText
    {
        public const string NoTableName = "没有配置TableName特性";
        public const string NotSupportedOperation = "不支持的操作在表达式处理中";
        public const string NotSupportedUnaryOperation = "不支持的操作在一元操作处理中";
        public const string NotSupportedFunctionOperation = "不支持的函数操作";
        public const string NotSupportedBinaryOperation = "不支持的操作在二元操作处理中";
        public const string ExpressionCantBeEmpty = "表达式不能为空";
        public const string ListEmpty = "列表为空";
        public const string NotSupportedDataBase = "不支持的数据库";
        public const string NotSupportedCache = "不支持的缓存";
        public const string NoTableWhenLinking = "联表时还没有联接";
    }

    public static class ExFunctions
    {
        /// <summary>
        /// 获取枚举值的描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumitem"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this T enumitem)
        {
            var item = enumitem.GetType().GetField(enumitem.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
            var result = ((DescriptionAttribute)item[0]).Description;
            return result;
        }
    }
}
