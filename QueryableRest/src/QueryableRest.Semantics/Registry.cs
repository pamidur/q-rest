using QueryableRest.Semantics.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryableRest.Semantics
{
    public class Registry
    {
        public IReadOnlyDictionary<string, IOperation> Operations => new Dictionary<string, IOperation>
        {
            { EqualOperation.DefaultMoniker, new EqualOperation() },
            { NotOperation.DefaultMoniker, new NotOperation() },
            { FilterOperation.DefaultMoniker, new FilterOperation() }
        };

    }
}
