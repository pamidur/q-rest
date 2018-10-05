using QRest.Core.Contracts;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public abstract class TermBase : ITerm
    {    
        public sealed override string ToString()
        {
            return $"{DebugView}";
        }

        public virtual string DebugView => $"#{GetType().Name.ToLowerInvariant().Replace("term", "")}";

        public abstract Expression CreateExpression(ICompilationContext compiler, Expression prev, ParameterExpression root);
        public abstract ITerm Clone();
    }
}
