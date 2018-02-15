using System;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class ConstantTerm : TermBase
    {
        public object Value { get; set; }

        protected override Expression CreateExpression(Expression prev, ParameterExpression root)
        {
            return Expression.Constant(Value, Value.GetType());
        }

        protected override string Debug
        {
            get
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

                return $"{del_l}{Value.ToString()}{del_r}";
            }
        }
    }
}
