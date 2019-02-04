using Microsoft.OData.Edm;
using System;

namespace QRest.Semantics.OData.Metadata
{
    static class EdmExtensions
    {
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
