using QueryableRest.Semantics.Arguments;
using QueryableRest.Semantics.Operations;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.SortOperations
{
    internal class OrderByDescendingOperation<TTarget> : SortOperation<TTarget>
    {
        public OrderByDescendingOperation(Operation<TTarget> sortKey) : base(sortKey)
        {
        }

        public override bool IsAscending => false;

        public override Expression CreateExpression(Expression parent)
        {
            var elementType = GetQueryElementType(parent);

            var param = Expression.Parameter(elementType, "r");
            var lambda = Expression.Lambda(Operand.CreateExpression(param), param);

            var orderByAscendingCall = Expression.Call(typeof(Queryable), "OrderByDescending", new[] { elementType, lambda.ReturnType }, parent, lambda);

            return orderByAscendingCall;
        }
    }
}