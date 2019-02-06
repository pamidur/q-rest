using QRest.Core.Operations;

namespace QRest.Semantics.OData.Parsing
{
    public class ODataOperationMap
    {
        public IOperation Select { get; set; } = OperationsMap.Map;
        public IOperation Where { get; set; } = OperationsMap.Where;
        public IOperation Order { get; set; } = OperationsMap.Order;
        public IOperation Skip { get; set; } = OperationsMap.Skip;
        public IOperation Top { get; set; } = OperationsMap.Take;
        public IOperation Count { get; set; } = OperationsMap.Count;
    }
}
