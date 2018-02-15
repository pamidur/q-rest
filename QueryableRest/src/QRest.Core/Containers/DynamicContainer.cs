using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QRest.Core.Containers
{
    internal class DynamicContainer : DynamicObject
    {
        public static readonly MethodInfo ArgAddMethod = ((Action<string, object>)new Dictionary<string, object>().Add).Method;
        public static readonly Type Type = typeof(DynamicContainer);
        public static readonly Type ArgType = typeof(Dictionary<string, object>);

        public static readonly ConstructorInfo TypeCtor = typeof(DynamicContainer).GetConstructor(new[] { ArgType });

        private IDictionary<string, object> _props = new Dictionary<string, object>();

        public DynamicContainer(IDictionary<string, object> initial)
        {
            _props = initial;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _props.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_props.ContainsKey(binder.Name))
            {
                result = _props[binder.Name];
                return true;
            }
            else
            {
                // todo:: check if need throw exception
                return base.TryGetMember(binder, out result);
            }

        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return base.TrySetMember(binder, value);
        }


        public static Expression CreateContainer(IReadOnlyDictionary<string, Expression> properties)
        {
            var initializers = properties.Select(p => Expression.ElementInit(ArgAddMethod, Expression.Constant(p.Key), Expression.Convert(p.Value, typeof(object))));
            var createContainer = Expression.New(TypeCtor, Expression.ListInit(Expression.New(ArgType), initializers));
            return createContainer;
        }

        public static Expression CreateReadProperty(Expression context, string name)
        {
            var ed = Expression.Dynamic(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, context.Type, new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }), typeof(object), context);

            return ed;
        }

        public static bool IsContainerType(Type type)
        {
            return type == Type;
        }
    }
}
