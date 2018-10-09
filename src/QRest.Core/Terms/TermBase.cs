using QRest.Core.Contracts;
using QRest.Core.Operations;
using QRest.Core.Operations.Basic;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public abstract class TermBase : ITerm
    {
        private static readonly IOperation _default = new NopOperation();

        public virtual IOperation Operation { get; protected set; } = _default;
        public virtual IReadOnlyList<ITermSequence> Arguments { get; protected set; } = new List<ITermSequence>();

        public abstract string SharedView { get; }
        public virtual string DebugView => SharedView;
        public virtual string KeyView => SharedView;

        public sealed override string ToString() => SharedView;

        public abstract ITerm Clone();
    }
}
