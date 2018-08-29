using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Containers
{
    public class StaticContainer
    {
        private readonly static Type _valueContainerTypeGeneric = typeof(Value<>);
        private readonly static Type _valueContainerType = typeof(Value);

        public readonly static Type ContainerType = typeof(Value[]);

        public class Value
        {            
            public object V { get; protected set; }
        }

        public class Value<T> : Value
        {
            public new T V
            {
                get => (T)base.V;
                set => base.V = value;
            }
        }

        public static Expression CreateContainer(IReadOnlyList<Expression> properties)
        {
            var initializers = new List<Expression>();

            foreach (var property in properties)
            {
                var contType = _valueContainerTypeGeneric.MakeGenericType(property.Type);
                var accessor = contType.GetProperty(nameof(Value<object>.V),property.Type).SetMethod;
                initializers.Add(Expression.MemberInit(Expression.New(contType),Expression.Bind(accessor,property)));
            }
            
            var createContainer = Expression.NewArrayInit(_valueContainerType, initializers);
            return createContainer;
        }

        public static Expression CreateReadProperty(Expression context, int index)
        {
            var getExp = Expression.Property(Expression.ArrayIndex(context,Expression.Constant(index)), nameof(Value.V));
            return getExp;
        }

        //public static bool IsContainerType(Type type)
        //{
        //    return type == Type;
        //}
    }
}
