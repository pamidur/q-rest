using QRest.Core.Contracts;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public abstract class TermBase : ITerm
    {    
        public ITerm Next { get; set; }  

        public sealed override string ToString()
        {
            return $"{DebugView}{Next?.ToString()}";
        }

        public virtual string DebugView => $"#{GetType().Name.ToLowerInvariant().Replace("term", "")}";

        public abstract Expression CreateExpression(ICompilerContext compiler, Expression prev, ParameterExpression root);
        public abstract ITerm Clone();
    }
}
