using QRest.Core.Contracts;
using QRest.Core.Expressions;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class NameTerm : TermBase
    {
        public string Name { get; set; }

        public override Expression CreateExpression(ICompilerContext compiler, Expression prev, ParameterExpression root)
        {
            return new NamedExpression(prev, Name);
        }

        public override string DebugView => $"@{Name}";

        public override ITerm Clone() => new NameTerm { Name = Name, Next = Next?.Clone() };
    }
}
