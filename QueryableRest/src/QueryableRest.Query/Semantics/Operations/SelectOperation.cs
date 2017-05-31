using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryableRest.Query.Semantics.Operations
{
    public class SelectOperation : Operation
    {
        public override string[] Monikers { get; } = new[] { "" };

        public override Expression CreateExpression(Expression query, Argument argument)
        {
            var elementType = GetQueryElementType(query);

            var param = Expression.Parameter(elementType, "r");
            var lambda = Expression.Lambda(argument.CreateExpression(param), param);

            var orderByAscendingCall = Expression.Call(typeof(Queryable), "OrderBy", new[] { elementType, lambda.ReturnType }, query, lambda);

            return orderByAscendingCall;
        }
    }
}