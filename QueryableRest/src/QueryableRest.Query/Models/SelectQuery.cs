using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryableRest.Query.Models
{
    public class SelectQuery<TResource> : SelectQuery
        where TResource : class
    {
        private static MethodInfo _addMethod = typeof(IDictionary<string, object>).GetMethod("Add");

        private static ConcurrentDictionary<Tuple<Type, string>, MethodCallExpression> _expressionCache = new ConcurrentDictionary<Tuple<Type, string>, MethodCallExpression>();

        public IQueryable ApplyTo(IQueryable<TResource> resources)
        {
            IQueryable result = resources; //todo:: fix this thing for EF6, works good in EF7

            if (Fields.Any())
            {
                var key = new Tuple<Type, string>(typeof(TResource), string.Join(";", Fields.OrderBy(f => f)));

                MethodCallExpression methodCall;
                if (!_expressionCache.TryGetValue(key, out methodCall))
                {
                    var resource = Expression.Parameter(typeof(TResource));
                    var param = Expression.Variable(typeof(ExpandoObject));

                    var selectExpressions =
                        new Expression[] { Expression.Assign(param, Expression.New(typeof(ExpandoObject))) }
                        .Concat(Fields.Select(f =>
                        {
                            Expression property = Expression.Property(resource, f);

                            if (property.Type.GetTypeInfo().IsValueType)
                                property = Expression.Convert(property, typeof(object));

                            return Expression.Call(param, _addMethod, Expression.Constant(f), property);
                        }))
                        .Concat(new Expression[] { param });

                    var selectLambda = Expression.Lambda(Expression.Block(new[] { param }, selectExpressions), resource);

                    methodCall = Expression.Call(typeof(Queryable), "Select", new[] { typeof(TResource), typeof(ExpandoObject) }, Expression.Constant(new List<TResource>().AsQueryable()), selectLambda);

                    _expressionCache[key] = methodCall;
                }

                result = result.Provider.CreateQuery(Expression.Call(methodCall.Method, result.Expression, methodCall.Arguments[1]));
            }

            return result;
        }
    }

    public abstract class SelectQuery
    {
        public SelectQuery()
        {
            Fields = new List<string>();
        }

        public List<string> Fields { get; }
    }
}