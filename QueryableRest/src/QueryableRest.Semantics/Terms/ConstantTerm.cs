using System;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Terms
{
    public class ConstantTerm : ITerm
    {
        public object Value { get; set; }
        public ITerm Next { get; set; }

        public Expression CreateExpression(Expression context, Expression root, Registry registry)
        {
            var exp = Expression.Constant(Value, Value.GetType());
            return Next?.CreateExpression(exp, root, registry) ?? exp;
        }

        public override string ToString()
        {
            var type = Value.GetType();

            var del_r = "";
            var del_l = "";

            if (type == typeof(string))
            {
                del_r = "'";
                del_l = "'";
            }
            else if (type == typeof(Guid))
            {
                del_r = "}";
                del_l = "{";
            }

            return $"{del_l}{Value.ToString()}{del_r}{Next?.ToString()}";
        }
    }
}
