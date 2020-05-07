using System.Collections.Immutable;
using System.Linq.Expressions;
using QRest.Core.Compilation.Containers;
using QRest.Core.Compilation.Expressions;
using QRest.Core.Compilation.TypeConverters;

namespace QRest.Core.Compilation.Visitors
{
    public class AssemblerServices : IAssembler
    {
        public AssemblerServices(IContainerFactory containerFactory, ITypeConverter typeConverter)
        {
            ContainerFactory = containerFactory;
            TypeConverter = typeConverter;
        }

        public IContainerFactory ContainerFactory { get; }
        public ITypeConverter TypeConverter { get; }


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

    public readonly struct AssemblerState
    {
        private AssemblerState(
            ParameterExpression root, Expression result, Expression context,
            in ImmutableArray<(ParameterExpression Param, ConstantExpression Value)> constants,
            AssemblerServices services)
        {
            Root = root;
            Result = result;
            Context = context;
            Constants = constants;
            Services = services;
        }

        public readonly Expression Context;
        public readonly Expression Result;
        public readonly ParameterExpression Root;
        public readonly ImmutableArray<(ParameterExpression Param, ConstantExpression Value)> Constants;
        public readonly AssemblerServices Services;

        public static AssemblerState New(ParameterExpression root, AssemblerServices services)
        {
            return new AssemblerState(root, root, root, in ImmutableArray<(ParameterExpression Param, ConstantExpression Value)>.Empty, services);
        }

        public AssemblerState Fork(ParameterExpression root)
        {
            return new AssemblerState(root, root, root, in ImmutableArray<(ParameterExpression Param, ConstantExpression Value)>.Empty, Services);
        }

        public AssemblerState Fork()
        {
            return new AssemblerState(Root, Result, Root, in ImmutableArray<(ParameterExpression Param, ConstantExpression Value)>.Empty, Services);
        }

        public AssemblerState Merge(in AssemblerState state)
        {
            return new AssemblerState(Root, Result, Context, Constants.AddRange(state.Constants), Services);
        }

        public AssemblerState WithResult(Expression result)
        {
            return new AssemblerState(Root, result, result, in Constants, Services);
        }
        public AssemblerState WithContext(Expression context)
        {
            return new AssemblerState(Root, Result, context, in Constants, Services);
        }
        public AssemblerState WithConstant(ConstantExpression constant, ParameterExpression parameter)
        {
            return new AssemblerState(Root, Result, Context, Constants.Add((parameter, constant)), Services);
        }
    }
}
