using QRest.Core.Contracts;
using QRest.Core.Expressions;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class NameTerm : TermBase
    {
        public string Name { get; set; }

        public override Expression CreateExpression(ICompilationContext compiler, Expression prev, ParameterExpression root)
        {
            return new NamedExpression(prev, Name);
        }

        public override string DebugView => $"@{Name}";

        public override ITerm Clone() => new NameTerm { Name = Name };
    }
}
