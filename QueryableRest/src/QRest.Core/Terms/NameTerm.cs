using QRest.Core.Expressions;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class NameTerm : ITerm
    {
        public ITerm Next { get; set; }
        public string Name { get; set; }

        public Expression CreateExpression(Expression prev, ParameterExpression root)
        {
            return new NamedExpression(prev, Name);
        }
    }
}
