using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Learn.Core.Util
{
    public static class DescriptionText
    {
        public const string Binary = "二元运算符";
        public const string Unary = "一元运算符";
        public const string Constant = "常量";
        public const string MemberAccess = "成员（变量）";
        public const string Function = "函数";
        public const string Unknown = "未知";
        public const string NotSupported = "不支持";
    }

    public enum EnumNodeType
    {
        [Description("二元运算符")]
        BinaryOperator = 1,
        [Description("一元运算符")]
        UndryOperator = 2,
        [Description("常量表达式")]
        Constant = 3,
        [Description("成员（变量）")]
        MemberAccess = 4,
        [Description("函数")]
        Call = 5,
        [Description("未知")]
        Unknown = -99,
        [Description("不支持")]
        NotSupported = -98
    }


}
