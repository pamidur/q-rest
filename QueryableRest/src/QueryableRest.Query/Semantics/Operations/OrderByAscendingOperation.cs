using System.Linq;
using System.Linq.Expressions;

namespace QueryableRest.Query.Semantics.SortOperations
{
    public class OrderByAscendingOperation : Operation
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