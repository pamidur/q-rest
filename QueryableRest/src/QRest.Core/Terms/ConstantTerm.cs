using System;
using System.Linq.Expressions;
using QRest.Core.Contracts;

namespace QRest.Core.Terms
{
    public class ConstantTerm : TermBase
    {
        public object Value { get; set; }

        public override Expression CreateExpression(ICompilerContext compiler, Expression prev, ParameterExpression root)
        {
            return Expression.Constant(Value, Value.GetType());
        }

        public override string DebugView
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

        public override ITerm Clone() => new ConstantTerm { Value = Value, Next = Next?.Clone() };
    }
}
