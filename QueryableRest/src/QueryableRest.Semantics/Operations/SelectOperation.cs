using QRest.Core.Containers;
using QRest.Core.Expressions;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QRest.Core.Operations
{
    public class SelectOperation : IOperation
    {
        public Expression CreateExpression(Expression last, ParameterExpression root, IReadOnlyList<Expression> arguments, QueryContext context)
        {
            var initializers = new Dictionary<string, Expression>();

            foreach (var arg in arguments)
            {
                var name = GetName(arg, context);

                if (string.IsNullOrEmpty(name) || initializers.ContainsKey(name))
                    name = CreateUniquePropName(initializers);

                initializers.Add(name, arg);
            }

            var createContainer = initializers.Any() ? context.Registry.ContainerProvider.CreateContainer(initializers) : root;

            var resultExpression = createContainer;

            if (typeof(IQueryable<>).MakeGenericType(root.Type).IsAssignableFrom(last.Type))
            {
                var lambda = Expression.Lambda(createContainer, root);

                resultExpression =
                    Expression.Call(typeof(Queryable), "AsQueryable", new[] { createContainer.Type },
                        Expression.Call(typeof(Enumerable), "AsEnumerable", new[] { createContainer.Type },
                            Expression.Call(typeof(Queryable), "Select", new Type[] { root.Type, createContainer.Type }, last, lambda)
                        )
                    );
            }

            var resultExpressionName = GetName(last, context) ?? "Select";

            return new NamedExpression(resultExpression, resultExpressionName); 
        }

        private string GetName(Expression arg, QueryContext context)
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
            else if (context.Registry.ContainerProvider.IsContainerType(arg.Type))
            {
                name = null;
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
