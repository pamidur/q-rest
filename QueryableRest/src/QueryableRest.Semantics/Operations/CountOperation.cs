using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QRest.Core.Operations
{
    public class CountOperation : IOperation
    {
        public Expression CreateExpression(Expression last, ParameterExpression root, IReadOnlyList<Expression> arguments, QueryContext context)
        {
            if (arguments.Count != 0)
                throw new ExpressionCreationException();

            if (!typeof(IQueryable<>).MakeGenericType(root.Type).IsAssignableFrom(last.Type))
                throw new ExpressionCreationException();

            return Expression.Call(typeof(Queryable), "Count", new Type[] { root.Type }, last);
        }
    }
}
