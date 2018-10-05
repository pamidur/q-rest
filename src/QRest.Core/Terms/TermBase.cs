using QRest.Core.Contracts;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public abstract class TermBase : ITerm
    {
        public abstract string SharedView { get; }
        public virtual string DebugView => SharedView;
        public virtual string KeyView => SharedView;
        public sealed override string ToString() => SharedView;

        public abstract Expression CreateExpression(ICompilationContext compiler, Expression prev, ParameterExpression root);
        public abstract ITerm Clone();
    }
}
