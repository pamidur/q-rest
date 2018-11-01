using QRest.Compiler.Standard.Containers;
using QRest.Core;
using QRest.Core.Expressions;
using System;
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

        private Expression QueryDynamic(Expression last, ParameterExpression root, IReadOnlyDictionary<string, Expression> fields)
        {
            var expression = fields.Any() ? DynamicContainer.CreateContainer(fields) : root;
            var lambda = Expression.Lambda(expression, root);

            return Expression.Call(typeof(Queryable), nameof(Queryable.Select), new Type[] { root.Type, expression.Type }, last, lambda);
        }

        private Expression QueryNonDynamic(Expression last, ParameterExpression root, IReadOnlyDictionary<string, Expression> fields)
        {
            var valuesList = fields.Select(e => e.Value).ToList();
            var staticExpression = fields.Any() ? StaticContainer.CreateContainer(valuesList) : root;

            var staticLambda = Expression.Lambda(staticExpression, root);

            var resultExpression = Expression.Call(typeof(Queryable), nameof(Queryable.AsQueryable), new[] { staticExpression.Type },
                    Expression.Call(typeof(Enumerable), nameof(Enumerable.ToArray), new[] { staticExpression.Type },
                        Expression.Call(typeof(Queryable), nameof(Queryable.Select), new Type[] { root.Type, staticExpression.Type }, last, staticLambda)
                    )
                );

            if (!fields.Any())
                return resultExpression;

            var dynParam = Expression.Parameter(StaticContainer.ContainerType);
            var dynamicFields = fields.ToDictionary(f => f.Key, f => StaticContainer.CreateReadProperty(dynParam, valuesList.IndexOf(f.Value)));

            return QueryDynamic(resultExpression, dynParam, dynamicFields);
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
