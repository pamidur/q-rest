using System;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryableRest.Semantics.Arguments
{
    public class GetPropertyOperation<TTarget> : Operation<TTarget>
    {
        public PropertyInfo Property { get; protected set; }

        public override Expression CreateExpression(Expression parent)
        {
            return Expression.Property(parent, Property);
        }

        public override string Serialize()
        {
            if (Operand == null)
                return Property.Name;
            else
                return $"{Operand.Serialize()}.{Property.Name}";
        }

        public static GetPropertyOperation<TTarget> From(string propertyPath)
        {
            return new GetPropertyOperation<TTarget>() { Property = typeof(TTarget).GetTypeInfo().GetDeclaredProperty(propertyPath) };
        }

        public static GetPropertyOperation<TTarget> From(Expression<Func<TTarget, object>> propertySelector)
        {
            var body = propertySelector.Body;
            if (body.NodeType == ExpressionType.Convert)
                body = ((UnaryExpression)body).Operand;
            return new GetPropertyOperation<TTarget>() { Property = (PropertyInfo)((MemberExpression)body).Member };
        }
    }
}