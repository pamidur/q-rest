using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QRest.Core.Extensions
{
    internal static class ExpressionExtensions
    {
        private static readonly Type _queryableIface = typeof(IQueryable<>);
        private static readonly string _queryableIfaceName = $"{_queryableIface.Namespace}.{_queryableIface.Name}";

        public static Type GetQueryElementType(this Expression query)
        {
            var typeInfo = query.Type.GetTypeInfo();

            if (!typeInfo.IsGenericType || (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() != _queryableIface))
            {
                typeInfo = typeInfo.GetInterface(_queryableIfaceName)?.GetTypeInfo();
            }

            return typeInfo?.GetGenericArguments()[0];
        }

        public static Expression ReduceTo(this Expression expression, ExpressionType[] expressionTypes)
        {
            while (expression.CanReduce && !expressionTypes.Contains(expression.NodeType))
                expression = expression.Reduce();

            return expression;
        }
    }
}
