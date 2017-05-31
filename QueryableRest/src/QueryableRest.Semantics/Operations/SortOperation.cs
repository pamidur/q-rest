using QueryableRest.Semantics.Arguments;
using QueryableRest.Semantics.SortOperations;
using System;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Operations
{
    public abstract class SortOperation<TTarget> : Operation<TTarget>
    {
        public SortOperation(Operation<TTarget> sortKey)
        {
            Operand = sortKey;
        }

        public abstract bool IsAscending { get; }

        public override string Serialize()
        {
            var moniker = (this is OrderByAscendingOperation<TTarget>) ? "" : "-";
            return $"{moniker}{Operand.Serialize()}";
        }        

        public static SortOperation<TTarget> From(string serializedSort)
        {
            return serializedSort.StartsWith("-")
                ? (SortOperation<TTarget>)new OrderByDescendingOperation<TTarget>(GetPropertyOperation<TTarget>.From(serializedSort.Substring(1)))
                : new OrderByAscendingOperation<TTarget>(GetPropertyOperation<TTarget>.From(serializedSort));
        }

        public static SortOperation<TTarget> ByAscending(Expression<Func<TTarget, object>> propertySelector)
        {
            return new OrderByAscendingOperation<TTarget>(GetPropertyOperation<TTarget>.From(propertySelector));
        }

        public static SortOperation<TTarget> ByDescending(Expression<Func<TTarget, object>> propertySelector)
        {
            return new OrderByDescendingOperation<TTarget>(GetPropertyOperation<TTarget>.From(propertySelector));
        }
    }
}
