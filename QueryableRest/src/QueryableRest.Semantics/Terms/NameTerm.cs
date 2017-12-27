using System;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class NameTerm : ITerm
    {
        public ITerm Next { get; set; }
        public string Name { get; set; }        

        public Expression CreateExpression(Expression prev, ParameterExpression root, QueryContext context)
        {
            var copy = CreateCopy(prev);
            context.NamedExpressions.AddOrUpdate(Name, copy);
            return copy;
        }

        private Expression CreateCopy(Expression prev)
        {
            if(prev.NodeType == ExpressionType.Call)
            {
                var call = (MethodCallExpression)prev;
                return Expression.Call(call.Object, call.Method, call.Arguments);
            }

            if(prev.NodeType == ExpressionType.MemberAccess)
            {
                var member = (MemberExpression) prev;
                return Expression.PropertyOrField(member.Expression, member.Member.Name);                    
            }

            if(prev.NodeType == ExpressionType.Constant)
            {
                var constant = (ConstantExpression)prev;
                return Expression.Constant(constant.Value, constant.Type);
            }

            if(prev.NodeType == ExpressionType.New)
            {
                var nw = (NewExpression)prev;
                return Expression.New(nw.Constructor, nw.Arguments);
            }

            if(prev.NodeType == ExpressionType.Parameter)
            {
                //todo::make a workaround
                return prev;
            }            

            throw new NotSupportedException();
        }
    }
}
