using QRest.Core.Terms;
using System;

namespace QRest.AspNetCore.Contracts
{
    public interface ICompiler
    {
        Func<TRoot, object> Compile<TRoot>(ITerm sequence);
    }
}
