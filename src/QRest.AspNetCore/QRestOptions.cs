using QRest.Core.Contracts;
using QRest.Semantics.MethodChain;

namespace QRest.AspNetCore
{
    public class QRestOptions
    {
        public IQuerySemanticsProvider Semantics { get; set; } = new MethodChainSemantics();
    }
}
