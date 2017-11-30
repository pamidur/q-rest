using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryableRest.Semantics.Operations
{
    public class EveryOperation : IOperation
    {
        public static readonly string DefaultMoniker = "every";

        public Expression CreateExpression(Expression context, Expression root, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count < 1)
                throw new ExpressionCreationException();

            var body = arguments.Aggregate((e1, e2) => { return Expression.AndAlso(e1, e2); });

            return body;
        }

        public Expression GetArgumentsRoot(Expression context)
        {
            return context;
        }
    }
}
