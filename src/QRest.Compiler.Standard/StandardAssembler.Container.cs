using QRest.Core;
using QRest.Core.Expressions;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard
{
    public partial class StandardAssembler
    {
        public IReadOnlyDictionary<string, Expression> GetInitializers(IReadOnlyList<Expression> arguments)
        {
            var fields = new Dictionary<string, Expression>();
            foreach (var arg in arguments)
            {
                var name = GetName(arg);
                if (string.IsNullOrEmpty(name) || fields.ContainsKey(name))
                    name = CreateUniquePropName(fields);
                fields.Add(name, arg);
            }

            return fields;
        }

        public LambdaExpression CreateContainer(ParameterExpression root, IReadOnlyDictionary<string, Expression> fields)
        {
            var lambda = Expression.Lambda(DynamicContainer.CreateContainer(fields), root);
            return lambda;
        }

        private string GetName(Expression arg)
        {
            string name = null;

            if (arg.NodeType == NamedExpression.NamedExpressionType)
            {
                name = ((NamedExpression)arg).Name;
            }
            else if (arg.NodeType == ExpressionType.MemberAccess)
            {
                name = ((MemberExpression)arg).Member.Name;
            }
            else if (arg.NodeType == ExpressionType.Parameter)
            {
                name = ((ParameterExpression)arg).Name;
            }
            else if (arg.NodeType == ExpressionType.Index)
            {
                var index = (IndexExpression)arg;
                var indexArg = index.Arguments.FirstOrDefault();
                if (indexArg == null)
                    throw new ExpressionCreationException();

                if (indexArg.NodeType != ExpressionType.Constant)
                    throw new ExpressionCreationException();

                name = ((ConstantExpression)indexArg).Value.ToString();
            }
            else if (arg.NodeType == ExpressionType.Call)
            {
                var call = (MethodCallExpression)arg;
                name = call.Method.Name;
            }
            else if (arg.NodeType == ExpressionType.Dynamic)
            {
                var dyn = (DynamicExpression)arg;

                if (dyn.Binder is GetMemberBinder)
                    name = ((GetMemberBinder)dyn.Binder).Name;
                else
                    throw new ExpressionCreationException();
            }
            else if (DynamicContainer.IsContainerType(arg.Type))
            {
                throw new ExpressionCreationException();
            }

            return name;
        }

        private string CreateUniquePropName(Dictionary<string, Expression> initializers)
        {
            var namePrefix = "Data";
            var ind = 0;

            var name = "";

            do
            {
                name = $"{namePrefix}{ind++}";
            } while (initializers.ContainsKey(name));

            return name;
        }
    }
}
