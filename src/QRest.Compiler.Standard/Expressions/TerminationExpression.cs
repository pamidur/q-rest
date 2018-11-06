using QRest.Core.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Expressions
{
    public class TerminationExpression : Expression
    {
        public static readonly ExpressionType ExpressionNodeType = (ExpressionType)2100;
        private readonly Type _element;
        private readonly Type _type;

        private TerminationExpression(Expression expression, Type element)
        {
            Expression = expression;
            _element = element;
            _type = typeof(IQueryable<>).MakeGenericType(_element);
        }

        public Expression Expression { get; }
        public override ExpressionType NodeType => ExpressionNodeType;
        public override Type Type => _type;
        public override bool CanReduce => true;

        public override Expression Reduce()
        {
            return
                Call(typeof(Queryable), nameof(Queryable.AsQueryable), new[] { _element }
                , Call(typeof(Enumerable), nameof(Enumerable.ToArray), new[] { _element }, Expression));
        }

        public override string ToString()
        {
            return $"{Expression.ToString()}.Terminate()";
        }

        public static Expression Create(Expression expression)
        {
            if (expression.NodeType == ExpressionNodeType)
                return expression;

            var element = expression.GetQueryElementType();
            if (element == null)
                return expression;

            return new TerminationExpression(expression, element);
        }
    }
}
