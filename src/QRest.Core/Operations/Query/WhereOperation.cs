using QRest.Core.Compilation;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public sealed class WhereOperation : LambdaOperationBase
    {
        internal WhereOperation() { }

        public override string Key { get; } = "where";

        protected override Expression CreateExpression(Expression context, Type collection, Type element, LambdaExpression argument, IAssembler assembler)
        {
            var exp = Expression.Call(collection, nameof(Queryable.Where), new Type[] { element }, context, argument);
            return assembler.SetName(exp, "data");
        }       
    }
}
