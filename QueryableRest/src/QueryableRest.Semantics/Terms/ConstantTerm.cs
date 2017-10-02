using System.Linq.Expressions;

namespace QueryableRest.Semantics.Terms
{
    public class ConstantTerm : ITerm
    {
        public object Value { get; set; }        

        public Expression CreateExpression(Expression context, Registry registry)
        {
            return Expression.Constant(Value, Value.GetType());
        }
    }
}
