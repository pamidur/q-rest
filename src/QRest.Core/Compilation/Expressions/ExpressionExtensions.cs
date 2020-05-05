using System.Linq.Expressions;

namespace QRest.Core.Compilation.Expressions
{
    internal static class ExpressionExtensions
    {
        public static Expression StripProxy(this Expression expression)
        {
            while (expression is ProxyExpression proxy)
                expression = proxy.OriginalExpression;

            return expression;
        }
    }
}
