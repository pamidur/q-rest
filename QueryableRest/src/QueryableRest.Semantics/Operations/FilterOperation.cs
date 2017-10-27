using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryableRest.Semantics.Operations
{
    public class FilterOperation : IOperation
    {
        public static readonly string DefaultMoniker = "filter";

        public Expression CreateExpression(Expression context, Expression argumentsRoot, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count < 1)
                throw new ExpressionCreationException();

            var body = arguments.Aggregate((e1, e2) => { return Expression.AndAlso(e1, e2); });

            var labmda = Expression.Lambda(body, (ParameterExpression) argumentsRoot);

            return Expression.Call(typeof(Queryable), "Where", new Type[] { argumentsRoot.Type }, context, labmda);
        }

        public Expression GetArgumentsRoot(Expression context)
        {
            return Expression.Parameter(GetQueryElementType(context));
        }

        protected Type GetQueryElementType(Expression query)
        {
            var typeInfo = query.Type.GetTypeInfo();

            if (typeInfo.FullName != "System.Linq.IQueryable`1")
            {
                typeInfo = typeInfo.GetInterface("System.Linq.IQueryable`1").GetTypeInfo();
            }

            return typeInfo.GetGenericArguments()[0];
        }
    }
}
