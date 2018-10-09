using System.Collections.Generic;

namespace QRest.Core.Contracts
{
    public interface ITermSequence : IEnumerable<ITerm>
    {
        IOperation RootSelector { get; }

        ITerm Root { get; }
        ITerm Last { get; }

        bool IsEmpty { get; }

        void Add(ITerm term);
        void Add(ITermSequence terms);

        string DebugView { get; }
        string SharedView { get; }
        string KeyView { get; }

        ITermSequence Clone();
    }
}
