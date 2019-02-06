using QRest.Core.Operations;
using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Query;
using System;
using System.Collections.Generic;

namespace QRest.Semantics.OData.Parsing
{
    public class ODataOperationOrder : IComparer<IOperation>
    {
        private readonly Dictionary<IOperation, int> _order;

        public ODataOperationOrder(ODataOperationMap operations)
        {
            _order = new Dictionary<IOperation, int>
            {
                {operations.Where, 0 },
                {operations.Count, 1 },
                {operations.Order, 2 },
                {operations.Skip, 3 },
                {operations.Top, 4 },
                {operations.Select, 5 }
            };
        }        

        public int Compare(IOperation x, IOperation y)
        {
            return _order[x] - _order[y];
        }
    }
}
