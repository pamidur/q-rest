using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Operations
{
    public class SelectOperation : IOperation
    {
        public static readonly string DefaultMoniker = "select";

        public Expression CreateExpression(Expression context, Expression argumentsRoot, IReadOnlyList<Expression> arguments)
        {
            if (!typeof(IQueryable<>).MakeGenericType(argumentsRoot.Type).IsAssignableFrom(context.Type))
                throw new ExpressionCreationException();            

            //new ExpandoObject()

            var lambda = Expression.Lambda(arguments[0], (ParameterExpression) argumentsRoot);

            return Expression.Call(typeof(Queryable), "Select", new Type[] { argumentsRoot.Type }, context, lambda);
        }        
    }
}
