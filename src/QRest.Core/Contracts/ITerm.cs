using System.Collections.Generic;

namespace QRest.Core.Contracts
{
    public interface ITerm
    {
        IOperation Operation { get; }
        IReadOnlyList<ITermSequence> Arguments { get; }

        ITerm Clone();

        string DebugView { get; }
        string SharedView { get; }
        string KeyView { get; }
    }
}