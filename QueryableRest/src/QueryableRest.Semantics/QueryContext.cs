using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core
{
    public class QueryContext
    {
        public IDictionary<string, Expression> NamedExpressions { get; set; } = new Dictionary<string, Expression>();
        public Registry Registry { get; set; }       
    }
}
