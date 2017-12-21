using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QRest.Core.Operations
{
    public class SumOperation : IOperation
    {
        public Expression CreateExpression(Expression last, ParameterExpression root, IReadOnlyList<Expression> arguments, QueryContext context)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            if (!typeof(IQueryable<>).MakeGenericType(root.Type).IsAssignableFrom(last.Type))
                throw new ExpressionCreationException();

            var lambda = Expression.Lambda(arguments[0], root);

            return Expression.Call(typeof(Queryable), "Sum", new Type[] { root.Type }, last, lambda);
        }
    }
}
