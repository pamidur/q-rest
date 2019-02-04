using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Core.Compilation
{
    public static class TypeExtensions
    {
        private static readonly Type _queryableIface = typeof(IQueryable<>);
        private static readonly Type _enumerableIface = typeof(IEnumerable<>);

        public static bool TryGetQueryableElement(this Type type, out Type element)
            => TryGetGenericElement(type, _queryableIface, out element);

        public static bool TryGetEnumerableElement(this Type type, out Type element)
            => TryGetGenericElement(type, _enumerableIface, out element);

        public static bool TryGetCollectionElement(this Type ctx, out (Type type, bool queryable) result)
        {
            result = default;

            if (ctx == typeof(string))
                return false;

            if (ctx.IsArray)
            {
                result = (ctx.GetElementType(), false);
                return true;
            }

            if (ctx.TryGetQueryableElement(out var element))
            {
                result = (element, true);
                return true;
            }

            if (ctx.TryGetEnumerableElement(out element))
            {
                result = (element, false);
                return true;
            }

            return false;
        }

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
