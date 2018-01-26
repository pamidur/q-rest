//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;

//namespace QueryableRest.Query.ModelBinders
//{
//    public abstract class QueryModelBinderBase
//    {
//        public readonly static string[] LexemSeparators = new[] { ";" };
//        public readonly static int PropertyDepth = 1;

//        protected static ConcurrentDictionary<Type, Dictionary<string, Type>> _fieldsCache = new ConcurrentDictionary<Type, Dictionary<string, Type>>();

//        protected static string ResolveFieldName(string name, Type resourceType, JsonSerializer serializer)
//        {
//            var props = GetFields(resourceType);
//            var resolvedFieldName = name;

//            if (serializer.ContractResolver is DefaultContractResolver)
//            {
//                var resolver = (DefaultContractResolver)serializer.ContractResolver;

//                var resolvedPropsMap = props.ToDictionary(
//                    p =>
//                    string.Join(".", p.Key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
//                    .Select(c => resolver.GetResolvedPropertyName(c)))
//                    , p => p.Key);

//                resolvedPropsMap.TryGetValue(resolvedFieldName, out resolvedFieldName);
//            }

//            if (!props.Keys.Any(k => k == resolvedFieldName))
//                throw new ApiException(ApiError.InvalidFieldName) { ErrorData = new { field = name } };

//            return resolvedFieldName;
//        }

//        protected static JsonSerializer GetSerializer(HttpConfiguration httpConfig)
//        {
//            var formatter = httpConfig.Formatters.JsonFormatter;
//            return JsonSerializer.Create(formatter.SerializerSettings);
//        }

//        protected static Type GetFieldType(string name, Type resourceType)
//        {
//            return GetFields(resourceType)[name];
//        }

//        protected static Dictionary<string, Type> GetFields(Type resourceType)
//        {
//            Dictionary<string, Type> result;

//            if (!_fieldsCache.TryGetValue(resourceType, out result))
//            {
//                result = ReadFields(null, resourceType).ToDictionary(p => p.Key, p => p.Value);
//                _fieldsCache[resourceType] = result;
//            }

//            return result;
//        }

//        protected static IEnumerable<KeyValuePair<string, Type>> ReadFields(string baseName, Type resourceType)
//        {
//            if (!string.IsNullOrEmpty(baseName))
//            {
//                if (baseName.Count(c => c == '.') == PropertyDepth)
//                    return new List<KeyValuePair<string, Type>>();

//                baseName += ".";
//            }

//            var result = resourceType.GetTypeInfo().GetProperties().ToDictionary(p => baseName + p.Name, p => p.PropertyType).ToList();

//            foreach (var prop in result.ToList())
//            {
//                if (!prop.Value.GetTypeInfo().IsValueType && !prop.Value.GetTypeInfo().IsPrimitive && prop.Value != typeof(string))
//                {
//                    result = result.Concat(ReadFields(prop.Key, prop.Value)).ToList();
//                }
//            }

//            return result;
//        }
//    }
//}