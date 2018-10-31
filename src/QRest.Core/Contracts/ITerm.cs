using System.Collections.Generic;

namespace QRest.Core.Contracts
{
    public interface ITerm
    {
        ITerm Clone { get; }
        string DebugView { get; }
        string SharedView { get; }
        string KeyView { get; }
    }
}