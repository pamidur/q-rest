using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using QRest.Core.Extensions;
using QRest.Core.Operations;

namespace QRest.Core
{
    public abstract class QueryBase
    {
        public ITerm RootTerm { get; set; } = new MethodTerm { Operation = new ItOperation() };

        public object Apply<T>(IQueryable<T> target, bool finalize = true)
        {
            var dataParam = Expression.Parameter(typeof(IQueryable<T>));

            var e = RootTerm.CreateExpressionChain(dataParam, dataParam);

            e = e.Reduce();            

            var l = Expression.Lambda(e, dataParam);            

            var r = l.Compile().DynamicInvoke(target);

            return r;
        }

    }
}
