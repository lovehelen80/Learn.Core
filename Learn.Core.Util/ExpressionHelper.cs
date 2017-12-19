using System;
using System.Linq.Expressions;

namespace Learn.Core.Util
{
    public class ExpressionHelper
    {
        public static string DealExpress(Expression exp)
        {
            if (exp is LambdaExpression)
            {
                LambdaExpression l_exp = exp as LambdaExpression;
                return DealExpress(l_exp.Body);
            }
            if (exp is BinaryExpression)
            {
                return DealBinaryExpression(exp as BinaryExpression);
            }
            if (exp is MemberExpression)
            {
                return DealMemberExpression(exp as MemberExpression);
            }
            if (exp is ConstantExpression)
            {
                return DealConstantExpression(exp as ConstantExpression);
            }
            if (exp is UnaryExpression)
            {
                return DealUnaryExpression(exp as UnaryExpression);
            }
            return "";
        }
        public static string DealUnaryExpression(UnaryExpression exp)
        {
            return DealExpress(exp.Operand);
        }
        public static string DealConstantExpression(ConstantExpression exp)
        {
            object vaule = exp.Value;
            string v_str = string.Empty;
            if (vaule == null)
            {
                return "NULL";
            }
            if (vaule is string)
            {
                v_str = string.Format("'{0}'", vaule.ToString());
            }
            else if (vaule is DateTime)
            {
                DateTime time = (DateTime)vaule;
                v_str = string.Format("'{0}'", time.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                v_str = vaule.ToString();
            }
            return v_str;
        }
        public static string DealBinaryExpression(BinaryExpression exp)
        {

            string left = DealExpress(exp.Left);
            string oper = GetOperStr(exp.NodeType);
            string right = DealExpress(exp.Right);
            if (right == "NULL")
            {
                if (oper == "=")
                {
                    oper = " is ";
                }
                else
                {
                    oper = " is not ";
                }
            }
            return left + oper + right;
        }
        public static string DealMemberExpression(MemberExpression exp)
        {
            return exp.Member.Name;
        }
        public static string GetOperStr(ExpressionType e_type)
        {
            switch (e_type)
            {
                case ExpressionType.OrElse: return " OR ";
                case ExpressionType.Or: return "|";
                case ExpressionType.AndAlso: return " AND ";
                case ExpressionType.And: return "&";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.Add: return "+";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.Divide: return "/";
                case ExpressionType.Modulo: return "%";
                case ExpressionType.Equal: return "=";
            }
            return "";
        }
    }
}
