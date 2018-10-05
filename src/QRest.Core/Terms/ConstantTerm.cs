using QRest.Core.Contracts;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class ConstantTerm : TermBase
    {
        public object Value { get; set; }

        public override Expression CreateExpression(ICompilationContext compiler, Expression prev, ParameterExpression root)
        {
            return Expression.Constant(Value, Value.GetType());
        }

        public override string SharedView => $"'{Value.ToString()}'";
        public override string KeyView => $"[{Value.GetType()}]";
        public override ITerm Clone() => new ConstantTerm { Value = Value };
    }
}
