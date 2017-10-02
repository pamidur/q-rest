using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QueryableRest.Semantics.Terms
{
    public class CallTerm : ITerm
    {
        public string Method { get; set; }
        public List<ITerm> Arguments { get; } = new List<ITerm>();
        public ITerm Parent { get; set; }

        public Expression CreateExpression(Registry registry)
        {
            throw new NotImplementedException();
        }
    }
}
