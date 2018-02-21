﻿using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class GreaterThanOperation : CompareOperationBase
    {
        protected override Expression PickExpression(Expression a, Expression b)=>
            Expression.GreaterThan(a, b);

    }
}
