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
        public Expression CreateExpression(Expression parent, ParameterExpression root, IReadOnlyList<Expression> arguments)
        {
            if (!typeof(IQueryable<>).MakeGenericType(root.Type).IsAssignableFrom(parent.Type))
                throw new ExpressionCreationException();


            var initializers = new List<ElementInit>();

            foreach (var arg in arguments)
            {
                string name;

                if (arg.NodeType == ExpressionType.MemberAccess)
                {
                    name = ((MemberExpression)arg).Member.Name;
                }
                else if (arg.NodeType == ExpressionType.Parameter)
                {
                    name = ((ParameterExpression)arg).Name;
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
                else throw new ExpressionCreationException();

                initializers.Add(Expression.ElementInit(PropertiesContainer.AddMethod, Expression.Constant(name), Expression.Convert(arg, typeof(object))));
            }

            var createContainer = Expression.ListInit(Expression.New(PropertiesContainer.Type), initializers);


            var lambda = Expression.Lambda(createContainer, root);

            return
                Expression.Call(typeof(Queryable), "AsQueryable", new[] { PropertiesContainer.Type },
                    Expression.Call(typeof(Enumerable), "AsEnumerable", new[] { PropertiesContainer.Type },
                        Expression.Call(typeof(Queryable), "Select", new Type[] { root.Type, PropertiesContainer.Type }, parent, lambda)
                    )
                );
        }
    }
}
