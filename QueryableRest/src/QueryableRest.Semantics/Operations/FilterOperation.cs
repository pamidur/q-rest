using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QueryableRest.Semantics.Operations
{
    public abstract class FilterOperation<TTarget> : Operation<TTarget>
    {
        public Operation<TTarget> Value { get; }

        public override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
