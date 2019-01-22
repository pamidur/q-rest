using Microsoft.OData.Edm;
using System;

namespace QRest.AspNetCore.OData.Metadata
{
    static class EdmExtensions
    {
        public static IEdmTypeReference MakeReference(this IEdmType type, bool nullable = true)
        {
            switch (type)
            {
                case IEdmEntityType resolvedType: return new EdmEntityTypeReference(resolvedType, nullable);
                case IEdmPrimitiveType resolvedType: return new EdmPrimitiveTypeReference(resolvedType, nullable);
            }

            throw new NotSupportedException();
        }
    }
}
