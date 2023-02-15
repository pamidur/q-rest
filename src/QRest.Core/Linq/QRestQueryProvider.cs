using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Linq
{
    public class QRestQuery<T> : IQueryable<T>, IOrderedQueryable<T>
    {
        public Type ElementType { get; } = typeof(T);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        public QRestQuery(QRestQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public IEnumerator<T> GetEnumerator()
            => Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }

    public class QRestQueryProvider : IQueryProvider
    {
        private readonly IExpressionToTermConverter _converter;
        private readonly ITermExecutor _executor;

        public QRestQueryProvider(IExpressionToTermConverter converter, ITermExecutor executor)
        {
            _converter = converter;
            _executor = executor;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) 
            => new QRestQuery<TElement>(this, expression);

        public IQueryable<TElement> CreateQuery<TElement>()
            => new QRestQuery<TElement>(this, new RootExpression(typeof(IQueryable<TElement>)));

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var term = _converter.Convert<TResult>(expression);
            var result = _executor.Execute<TResult>(term).Result;
            return result;
        }
    }
}
