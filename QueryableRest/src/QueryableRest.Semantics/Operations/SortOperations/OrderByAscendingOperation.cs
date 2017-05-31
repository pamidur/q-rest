using QueryableRest.Semantics.Arguments;
using QueryableRest.Semantics.Operations;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.SortOperations
{
    internal class OrderByAscendingOperation<TTarget> : SortOperation<TTarget>
    {
        public OrderByAscendingOperation(Operation<TTarget> sortKey) : base(sortKey)
        {
        }

        public override bool IsAscending => true;

        public override Expression CreateExpression(Expression parent)
        {
            var elementType = GetQueryElementType(parent);

            var param = Expression.Parameter(elementType, "r");
            var lambda = Expression.Lambda(Operand.CreateExpression(param), param);

            var orderByAscendingCall = Expression.Call(typeof(Queryable), "OrderBy", new[] { elementType, lambda.ReturnType }, parent, lambda);

            return orderByAscendingCall;
        }
    }
}