using System;

namespace QRest.Core.Contracts
{
    public interface ICompiler
    {
        Func<TRoot, object> Compile<TRoot>(ITerm sequence);
    }
}
