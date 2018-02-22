using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Query;
using System;
using System.Collections.Generic;

namespace QRest.Semantics.OData
{
    public class ODataOperationOrder : IComparer<Type>
    {
        public static IDictionary<Type, int> OperationOrder = new Dictionary<Type, int>
        {
            {typeof(WhereOperation), 0 },
            {typeof(CountOperation), 1 }

        };

        public int Compare(Type x, Type y)
        {
            return OperationOrder[x] - OperationOrder[y];
        }
    }
}
