using System;
using System.Linq.Expressions;
using System.Reflection;

namespace QRest.Core.Extensions
{
    internal static class ExpressionExtensions
    {
        public static Type GetQueryElementType(this Expression query)
        {
            var typeInfo = query.Type.GetTypeInfo();

            if ($"{typeInfo.Namespace}.{typeInfo.Name}" != "System.Linq.IQueryable`1")
            {
                typeInfo = typeInfo.GetInterface("System.Linq.IQueryable`1")?.GetTypeInfo();
            }

            return typeInfo?.GetGenericArguments()[0];
        }
    }
}
