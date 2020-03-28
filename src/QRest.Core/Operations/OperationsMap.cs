using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Core.Operations
{
    public static class OperationsMap
    {
        private static readonly IDictionary<string, IOperation> _lookup = new Dictionary<string, IOperation>();
        
        public static readonly IOperation Any = RegisterOperation(new AnyOperation());
        public static readonly IOperation All = RegisterOperation(new AllOperation());

        public static readonly IOperation Count = RegisterOperation(new CountOperation());
        public static readonly IOperation First = RegisterOperation(new FirstOperation());
        public static readonly IOperation Sum = RegisterOperation(new SumOperation());

        public static readonly IOperation Not = RegisterOperation(new NotOperation());
        public static readonly IOperation Has = RegisterOperation(new HasOperation());
        public static readonly IOperation Equal = RegisterOperation(new EqualOperation());
        public static readonly IOperation NotEqual = RegisterOperation(new NotEqualOperation());
        public static readonly IOperation GreaterThan = RegisterOperation(new GreaterThanOperation());
        public static readonly IOperation GreaterThanOrEqual = RegisterOperation(new GreaterThanOrEqualOperation());
        public static readonly IOperation LessThan = RegisterOperation(new LessThanOperation());
        public static readonly IOperation LessThanOrEqual = RegisterOperation(new LessThanOrEqualOperation());

        public static readonly IOperation Every = RegisterOperation(new EveryOperation());
        public static readonly IOperation OneOf = RegisterOperation(new OneOfOperation());

        public static readonly IOperation Map = RegisterOperation(new MapOperation());
        public static readonly IOperation Skip = RegisterOperation(new SkipOperation());
        public static readonly IOperation Take = RegisterOperation(new TakeOperation());
        public static readonly IOperation Where = RegisterOperation(new WhereOperation());
        public static readonly IOperation Order = RegisterOperation(new OrderOperation());
        public static readonly IOperation Reverse = RegisterOperation(new ReverseOrderOperation());

        public static readonly IOperation New = RegisterOperation(new NewOperation());


        public static IOperation RegisterOperation(IOperation operation)
        {
             _lookup.Add(operation.Key, operation);
            return operation;
        }

        public static IOperation LookupOperation(string name) => _lookup.TryGetValue(name, out var op) ? op : null;
        public static IReadOnlyList<string> GetRegisteredOperationNames() => _lookup.Keys.ToArray();
    }
}
