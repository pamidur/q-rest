using QRest.Core.Terms;
using System;

namespace QRest.Core.Contracts
{
    public interface ICompiler
    {
        Func<TRoot, object> Compile<TRoot>(TermSequence sequence, bool finalize = true);
    }
}
