using System.Collections.Generic;
using System.Linq.Expressions;
using QRest.Core.Compilation.Containers;
using QRest.Core.Compilation.Expressions;
using QRest.Core.Compilation.TypeConverters;

namespace QRest.Core.Compilation.Visitors
{
    public class AssemblerContext : IAssembler
    {
        public Expression Context { get; set; }
        public ParameterExpression Root { get; set; }

        public List<ConstantExpression> Constants { get; } = new List<ConstantExpression>();
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();

        public IContainerFactory ContainerFactory { get; set; }
        public ITypeConverter TypeConverter { get; set; }

        public string GetName(Expression arg)
        {
            if (arg is NamedExpression named) return named.Name;
            if (arg is UnaryExpression unary) return GetName(unary.Operand);
            if (arg is MemberExpression member) return member.Member.Name;
            if (arg is ParameterExpression parameter) return parameter.Name;
            if (arg is IndexExpression index && index.Arguments.Count == 1 && index.Arguments[0] is ConstantExpression constant) return constant.Value.ToString();
            //if (arg is DynamicExpression dynamic && dynamic.Binder is GetMemberBinder binder) return binder.Name;
            if (arg.CanReduce) return GetName(arg.Reduce());
            if (arg is MethodCallExpression call)
            {
                if (call.Object != null) return GetName(call.Object);
                else if (call.Arguments.Count != 0) return GetName(call.Arguments[0]);
            }
            return "data";
        }

        public Expression SetName(Expression expression, string name)
        {
            return NamedExpression.Create(expression, name);
        }
    }
}
