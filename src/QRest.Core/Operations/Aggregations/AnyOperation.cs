﻿using QRest.Core.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Aggregations
{
    public class AnyOperation : LambdaOperationBase
    {
        public override string Key { get; } = "any";

        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, bool queryable, LambdaExpression argument, IAssemblerContext assembler)
        {
            var host = queryable ? typeof(Queryable) : typeof(Enumerable);

            var exp = (Expression)Expression.Call(host, nameof(Queryable.Any), new Type[] { element }, new[] { context, argument });
            exp = Expression.AndAlso(Expression.NotEqual(context, Expression.Constant(null, context.Type)), exp);

            return assembler.SetName(exp, Key);
        }
    }
}
