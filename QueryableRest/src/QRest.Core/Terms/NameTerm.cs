using QRest.Core.Expressions;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class NameTerm : TermBase
    {
        public string Name { get; set; }

        protected override Expression CreateExpression(Expression prev, ParameterExpression root)
        {
            return new NamedExpression(prev, Name);
        }

        protected override string Debug => $"@{Name}";
    }
}
