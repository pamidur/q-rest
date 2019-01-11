using Microsoft.OData.Edm;
using System;
using System.Reflection;

namespace QRest.AspNetCore.OData
{
    public class MetadataBuilder
    {
        private readonly string _namespace;
        private readonly EdmModel _model;
        private readonly EdmEntityContainer _container;

        private MetadataBuilder(string @namespace)
        {
            _namespace = @namespace;
            _model = new EdmModel();
            _container = _model.AddEntityContainer(_namespace, "Container");
        }

        public static MetadataBuilder New(string @namespace)
        {
            return new MetadataBuilder(@namespace);
        }

        public MetadataBuilder Map(Type type)
        {
            var edmType = MapType(type);

            _container.AddEntitySet(type.Name + "Set", (IEdmEntityType) edmType);
            return this;
        }

        public IEdmModel Build()
        {
            return _model;
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
            var entityType = _model.AddEntityType(_namespace, type.Name);

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance))
            {
                var propTypeRef = MapType(prop.PropertyType).MakeReference();
                entityType.AddStructuralProperty(prop.Name, propTypeRef);
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
