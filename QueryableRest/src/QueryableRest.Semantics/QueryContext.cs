using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core
{
    public class QueryContext
    {
        public IDictionary<string, Expression> NamedExpressions { get; set; } = new Dictionary<string, Expression>();
        public Expression Root { get; set; }
        public Registry Registry { get; set; }

        public QueryContext ChangeRoot(Expression root)
        {
            return new QueryContext
            {
                //todo:: make named expressions inherit perv root context, but not override it
                NamedExpressions = NamedExpressions,
                Registry = Registry,
                Root = root
            };
        }
    }
}
