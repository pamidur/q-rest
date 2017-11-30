using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryableRest.Semantics
{
    public class QueryContext
    {
        public QueryContext(Expression root, Registry registry)
        {
            Root = root;
            Registry = registry;
        }

        public IDictionary<string, Expression> NamedExpressions { get; } = new Dictionary<string, Expression>();
        public Expression Root { get; }
        public Registry Registry { get; }
    }
}
