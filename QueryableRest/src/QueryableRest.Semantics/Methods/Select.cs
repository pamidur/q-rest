using QueryableRest.Semantics.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryableRest.Semantics.Methods
{
    public class Select<TTarget> : Method<TTarget, SelectOperation<TTarget>>
    {
        public override ICollection<SelectOperation<TTarget>> Operations => throw new NotImplementedException();
    }
}
