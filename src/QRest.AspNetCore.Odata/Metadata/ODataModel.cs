using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace QRest.AspNetCore.OData.Metadata
{
    public class ODataModel
    {
        private readonly string _namespace;
        private readonly EdmEntityContainer _container;

        public EdmModel Schema { get; }
        public IDictionary<string, IEdmEntityContainerElement> Registry { get; } = new Dictionary<string, IEdmEntityContainerElement>();

        private ODataModel(string @namespace)
        {
            _namespace = @namespace;
            Schema = new EdmModel();
            _container = Schema.AddEntityContainer(_namespace, "Container");
        }

        public static ODataModel New(string @namespace)
        {
            return new ODataModel(@namespace);
        }

        public ODataModel MapSet(Type type, string setName, string url)
        {
            if (Registry.ContainsKey(url))
                return this;

            var edmType = MapType(type);

            var set = _container.AddEntitySet(setName, (IEdmEntityType) edmType);
            Registry.Add(url, set);
            return this;
        }

        private IEdmType MapType(Type type)
        {
            if (type.IsPrimitive || type == typeof(string))
                return MapPrimitive(type);

            if (type.IsClass)            
                return MapClass(type);             

            throw new NotSupportedException();
        }

        private IEdmType MapClass(Type type)
        {
            var existing = Schema.FindDeclaredType($"{_namespace}.{type.Name}")?.AsActualType();

            if (existing != null)
                return existing;

            var entityType = Schema.AddEntityType(_namespace, type.Name);

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance))
            {
                var propTypeRef = MapType(prop.PropertyType).MakeReference();
                var propref = entityType.AddStructuralProperty(prop.Name, propTypeRef);               
            }

            return entityType;
        }

        private IEdmType MapPrimitive(Type type)
        {
            var code = Type.GetTypeCode(type);

            switch (code)
            {
                case TypeCode.Int32: return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
                case TypeCode.Int64: return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64);
                case TypeCode.Boolean: return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean);
                case TypeCode.Byte: return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte);
                case TypeCode.SByte: return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte);
                case TypeCode.Single: return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single);
                case TypeCode.Double: return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double);
                case TypeCode.DateTime: return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset);
                case TypeCode.String: return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String);
            }

            throw new NotImplementedException();
        }
    }
}
