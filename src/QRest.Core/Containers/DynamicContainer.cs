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
        public static readonly Type Type = typeof(DynamicContainer);

        private readonly static Type _valueContainerType = typeof(PropertyContainer);

        private IDictionary<string, object> _props = new Dictionary<string, object>();

        private static readonly MethodInfo _sourceSetter = Type.GetProperty(nameof(Source), typeof(PropertyContainer[])).SetMethod;
        private static readonly MethodInfo _nameSetter = _valueContainerType.GetProperty(nameof(PropertyContainer.N)).SetMethod;
        private static readonly MethodInfo _valueSetter = _valueContainerType.GetProperty(nameof(PropertyContainer.V)).SetMethod;
               

        public class PropertyContainer
        {
            public string N { get; set; }
            public object V { get; set; }
        }

        public DynamicContainer()
        {

        }

        public PropertyContainer[] Source
        {
            set
            {
                foreach (var prop in value)
                {
                    _props.Add(prop.N, prop.V);
                }
            }
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
            var initializers = new List<Expression>();

            foreach (var property in properties)
            {
                initializers.Add(Expression.MemberInit(Expression.New(typeof(PropertyContainer)), Expression.Bind(_valueSetter, Expression.Convert(property.Value,typeof(object))), Expression.Bind(_nameSetter, Expression.Constant(property.Key))));
            }

            var createContainer = Expression.MemberInit(Expression.New(Type),Expression.Bind(_sourceSetter, Expression.NewArrayInit(_valueContainerType, initializers)));
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
