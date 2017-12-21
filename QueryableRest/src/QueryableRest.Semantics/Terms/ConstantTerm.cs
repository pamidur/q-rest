using System;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class ConstantTerm : ITerm
    {
        public object Value { get; set; }
        public ITerm Next { get; set; }

        public Expression CreateExpression(Expression prev, ParameterExpression root, QueryContext context)
        {
            var exp = Expression.Constant(Value, Value.GetType());
            return Next?.CreateExpression(exp, root, context) ?? exp;
        }

        public override string ToString()
        {
            var type = Value.GetType();

            var del_r = "";
            var del_l = "";

            if (type == typeof(string))
            {
                del_r = "`";
                del_l = "`";
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
