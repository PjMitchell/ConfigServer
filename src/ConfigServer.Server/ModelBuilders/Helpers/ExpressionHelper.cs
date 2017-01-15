using System.Linq.Expressions;

namespace ConfigServer.Server
{
    internal class ExpressionHelper
    {
        public static string GetPropertyNameFromExpression(LambdaExpression expression)
        {
            return GetExpressionBody(expression).Member.Name;
        }

        public static MemberExpression GetExpressionBody(LambdaExpression expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            return body;
        }
    }
}
