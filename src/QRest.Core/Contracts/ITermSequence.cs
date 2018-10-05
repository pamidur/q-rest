using System.Collections.Generic;

namespace QRest.Core.Contracts
{
    public interface ITermSequence : IEnumerable<ITerm>, ITerm
    {
        ITerm Root { get; }
        ITerm Last { get; }

        bool IsEmpty { get; }

        void Add(ITerm term);
    }
}
