using QRest.Core.Compilation.Expressions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace QRest.Core.Compilation.Containers
{
    public class DynamicContainerFactory : IContainerFactory
    {
        private static readonly Type _type = typeof(DynamicContainer);
        private static readonly Type _valueContainerType = typeof(PropertyContainer);
        private static readonly Type _valueType = typeof(object);

        private static readonly PropertyInfo _indexer = _type.GetProperty("Item", typeof(object));
        private static readonly MethodInfo _sourceSetter = _type.GetProperty(nameof(DynamicContainer.Source), typeof(PropertyContainer[])).SetMethod;
        private static readonly MemberInfo _nameSetter = _valueContainerType.GetField(nameof(PropertyContainer.N));
        private static readonly MemberInfo _valueSetter = _valueContainerType.GetField(nameof(PropertyContainer.V));

        public Expression CreateContainer(IReadOnlyDictionary<string, Expression> properties)
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

        public Expression CreateReadProperty(Expression context, string name)
        {
            Expression ed = Expression.MakeIndex(context, _indexer, new[] { Expression.Constant(name) });

            if (context is ContainerExpression container)
                ed = Expression.Convert(ed, container.Properties[name].Type);

            return ed;
        }

        public bool IsContainerExpression(Expression expression)
        {
            return expression is ContainerExpression && expression.Type == _type;
        }
    }

#pragma warning disable CS0649
    internal class PropertyContainer
    {
        public string N;
        public object V;
    }
#pragma warning restore CS0649

    internal class DynamicContainer : DynamicObject
    {
        private IDictionary<string, object> _props = new Dictionary<string, object>();

        public PropertyContainer[] Source
        {
            set
            {
                foreach (var prop in value)
                    _props.Add(prop.N, prop.V);
            }
        }

        public object this[string name] => _props[name];

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
    }
}
