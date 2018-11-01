using QRest.Core.Terms;
using System;

namespace QRest.Core.Contracts
{
    public interface ICompiler
    {
        Func<TRoot, object> Compile<TRoot>(LambdaTerm sequence, bool finalize = true);
    }
}
