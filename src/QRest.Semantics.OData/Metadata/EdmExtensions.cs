using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Semantics.OData.Metadata
{
    static class EdmExtensions
    {
        public static bool IsDictionary(this Type type)
        {
            return new[] { type }.Concat(type.GetInterfaces())
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(g => g == typeof(IDictionary<,>) || g == typeof(IReadOnlyDictionary<,>));
        }

        public static IEdmTypeReference MakeReference(this IEdmType type, bool nullable = true)
        {
            switch (type)
            {
                case IEdmEntityType resolvedType: return new EdmEntityTypeReference(resolvedType, nullable);
                case IEdmPrimitiveType resolvedType: return new EdmPrimitiveTypeReference(resolvedType, nullable);
                case IEdmCollectionType resolvedType: return new EdmCollectionTypeReference(resolvedType);
                case IEdmComplexType resolvedType: return new EdmComplexTypeReference(resolvedType, nullable);
            }

            throw new NotSupportedException();
        }
    }
}
