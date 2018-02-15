using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QRest.Core
{
    public abstract class QueryBase
    {
        public ITerm RootTerm { get; set; }

        public object Apply<T>(IQueryable<T> target)
        {
            if (RootTerm == null)
                return target;

            var dataParam = Expression.Parameter(typeof(IQueryable<T>));

            var e = RootTerm.CreateExpression(dataParam, dataParam);

            e = e.Reduce();

            var l = Expression.Lambda(e, dataParam);            

            var r = l.Compile().DynamicInvoke(target);

            return r;
        }

    }
}
