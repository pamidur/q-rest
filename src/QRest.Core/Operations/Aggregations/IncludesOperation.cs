﻿using QRest.Core.Compilation;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Aggregations
{
    public sealed class IncludesOperation : LambdaOperationBase
    {
        internal IncludesOperation() { }

        public override string Key { get; } = "includes";

        protected override Expression CreateExpression(Expression context, Type collection, Type element, LambdaExpression argument, IAssembler assembler)
        {
            var exp = (Expression)Expression.Call(collection, nameof(Queryable.Any), new Type[] { element }, new[] { context, argument });
            return assembler.SetName(exp, Key);
        }
    }
}
