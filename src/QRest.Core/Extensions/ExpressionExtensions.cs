using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Extensions
{
    public static class ExpressionExtensions
    {
        private static readonly Type _queryableIface = typeof(IQueryable<>);
        private static readonly Type _enumerableIface = typeof(IEnumerable<>);
                     
        public static Expression ReduceTo(this Expression expression, ExpressionType[] expressionTypes)
        {
            while (expression.CanReduce && !expressionTypes.Contains(expression.NodeType))
                expression = expression.Reduce();

            return expression;
        }

        public static bool TryGetQueryableElement(this Type type, out Type element) 
            => TryGetGenericElement(type, _queryableIface, out element);

        public static bool TryGetEnumerableElement(this Type type, out Type element)
            => TryGetGenericElement(type, _enumerableIface, out element);

        public static bool TryGetGenericElement(this Type type, Type genericInterface, out Type element)
        {
            if (type.IsGenericType)
            {
                var def = type.GetGenericTypeDefinition();
                if (def == genericInterface)
                {
                    element = type.GetGenericArguments()[0];
                    return true;
                }
            }

            foreach (var iface in type.GetInterfaces())
            {
                if (TryGetGenericElement(iface, genericInterface, out element))
                    return true;
            }

            element = null;
            return false;
        }
    }
}
