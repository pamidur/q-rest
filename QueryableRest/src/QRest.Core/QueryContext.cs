using QRest.Core.Containers;
using QRest.Core.Contracts;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core
{
    public class QueryContext
    {
        public QueryContext()
        {

        }

        public IContainerProvider ContainerProvider { get; } = new DynamicContainerProvider();

        public QueryContext Derive()
        {
            return new QueryContext { };
        }
    }
}
