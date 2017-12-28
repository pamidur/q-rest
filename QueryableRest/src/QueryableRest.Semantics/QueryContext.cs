using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core
{
    public class QueryContext
    {
        private QueryContext()
        {

        }

        public QueryContext(Registry registry)
        {
            Registry = registry;
        }
        
        public Registry Registry { get; private set; }

        public QueryContext Derive()
        {
            return new QueryContext { Registry = Registry};
        }
    }
}
