using QRest.Compiler.Standard.Expressions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace QRest.Compiler.Standard.Assembler
{
    internal class DynamicContainer : DynamicObject
    {
        private static readonly Type _type = typeof(DynamicContainer);
        private static readonly Type _valueContainerType = typeof(PropertyContainer);
        private static readonly Type _valueType = typeof(object);

        private static readonly MethodInfo _sourceSetter = _type.GetProperty(nameof(Source), typeof(PropertyContainer[])).SetMethod;
        private static readonly MemberInfo _nameSetter = _valueContainerType.GetField(nameof(PropertyContainer.N));
        private static readonly MemberInfo _valueSetter = _valueContainerType.GetField(nameof(PropertyContainer.V));

#pragma warning disable CS0649 
        public class PropertyContainer
        {
            public string N;
            public object V;
        }
#pragma warning restore CS0649

        private IDictionary<string, object> _props = new Dictionary<string, object>();

        public PropertyContainer[] Source
        {
            set
            {
                foreach (var prop in value)
                    _props.Add(prop.N, prop.V);
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
                initializers.Add(Expression.MemberInit(Expression.New(typeof(PropertyContainer)),
                    Expression.Bind(_valueSetter, Expression.Convert(property.Value, typeof(object))),
                    Expression.Bind(_nameSetter, Expression.Constant(property.Key))));

            var createContainer = Expression.MemberInit(Expression.New(_type),
                Expression.Bind(_sourceSetter, Expression.NewArrayInit(_valueContainerType, initializers)));
            return ContainerExpression.Create(createContainer, properties);
        }

        public static Expression CreateReadProperty(Expression context, string name)
        {
            Expression ed = Expression.Dynamic(
                Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, context.Type,
                new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }), typeof(object), context);

            if (context is ContainerExpression container)
                ed = Expression.Convert(ed, container.Properties[name].Type);

            return ed;
        }

        public static bool IsContainerType(Type type)
        {
            return type == _type;
        }
    }
}
