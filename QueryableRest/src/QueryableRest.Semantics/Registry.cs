using QueryableRest.Semantics.Contracts;
using QueryableRest.Semantics.Conventions;
using QueryableRest.Semantics.Operations;
using System.Collections.Generic;

namespace QueryableRest.Semantics
{
    public class Registry
    {
        public IReadOnlyDictionary<string, IOperation> Operations { get; } = new Dictionary<string, IOperation>
        {
            { NotEqualOperation.DefaultMoniker, new NotEqualOperation() },
            { EqualOperation.DefaultMoniker, new EqualOperation() },
            { NotOperation.DefaultMoniker,   new NotOperation() },
            { WhereOperation.DefaultMoniker, new WhereOperation() },
            { SelectOperation.DefaultMoniker, new SelectOperation() },
            { OneOfOperation.DefaultMoniker, new OneOfOperation() },
            { EveryOperation.DefaultMoniker, new EveryOperation() },
        };

        public INameConvention PropertyNameConvention { get; } = new PascalCaseConvention();
        public INameConvention MethodNameConvention { get; } = new PascalCaseConvention();

    }
}
