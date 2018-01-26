using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public class WhereOperation : IOperation
    {
        public Expression CreateExpression(Expression last, ParameterExpression root, IReadOnlyList<Expression> arguments, QueryContext context)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            if (arguments[0].Type != typeof(bool))
                throw new ExpressionCreationException();

            if (!typeof(IQueryable<>).MakeGenericType(root.Type).IsAssignableFrom(last.Type))
                throw new ExpressionCreationException();                

            var lambda = Expression.Lambda(arguments[0], root);

            return Expression.Call(typeof(Queryable), "Where", new Type[] { root.Type }, last, lambda);
        }        
    }
}
