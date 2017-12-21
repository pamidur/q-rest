using QRest.Core.Containers;
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
                string name;

                if (context.NamedExpressions.Values.Any(v=>v==arg))
                {
                    name = context.NamedExpressions.First(p => p.Value == arg).Key;
                }
                else if (arg.NodeType == ExpressionType.MemberAccess)
                {
                    name = ((MemberExpression)arg).Member.Name;
                }                
                else if (arg.NodeType == ExpressionType.Parameter)
                {
                    name = ((ParameterExpression)arg).Name;
                    if (string.IsNullOrEmpty(name))
                        name = "Data";
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
                else if(arg.NodeType == ExpressionType.Call)
                {
                    var call = (MethodCallExpression)arg;
                    name = call.Method.Name;
                    if (name == "AsQueryable" || name == "AsEnumerable" || name == "ToList" || name == "ToArray")
                        name = "Data";
                }
                else if(arg.NodeType == ExpressionType.Dynamic)
                {
                    var dyn = (DynamicExpression)arg;

                    if (dyn.Binder is GetMemberBinder)
                        name = ((GetMemberBinder)dyn.Binder).Name;
                    else
                        throw new ExpressionCreationException();
                }
                else throw new ExpressionCreationException();

                initializers.Add(name, arg);
            }            

            var createContainer =  initializers.Any() ? context.Registry.ContainerProvider.CreateContainer(initializers) : root;

            if (typeof(IQueryable<>).MakeGenericType(root.Type).IsAssignableFrom(last.Type))
            {
                var lambda = Expression.Lambda(createContainer, root);

                return
                    Expression.Call(typeof(Queryable), "AsQueryable", new[] { createContainer.Type },
                        Expression.Call(typeof(Enumerable), "AsEnumerable", new[] { createContainer.Type },
                            Expression.Call(typeof(Queryable), "Select", new Type[] { root.Type, createContainer.Type }, last, lambda)
                        )
                    );
            }
            else
            {
                return createContainer;
            }            
        }
    }
}
