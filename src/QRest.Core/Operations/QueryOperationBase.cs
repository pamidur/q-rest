using QRest.Core.Compilation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QRest.Core.Operations
{
    public abstract class QueryOperationBase : OperationBase
    {
        private static readonly Type _queryableCollection = typeof(IQueryable<>);
        private static readonly MethodInfo _nonNullCollectionMethod = typeof(QueryOperationBase).GetMethod(nameof(NonNullCollection));        

        public override Expression CreateExpression(Expression context, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            if (!context.Type.TryGetCollectionElement(out var element))
                throw new CompilationException($"Cannot execute '{Key}' method on non-collection type '{context.Type}'.");
            
            //var type = _queryableCollection.MakeGenericType(element.type);
            //context = Expression.Convert(context, type, _nonNullCollectionMethod.MakeGenericMethod(element.type));

            return CreateExpression(context, GetQueryableType(context), element.type, arguments, assembler);
        }       

        protected abstract Expression CreateExpression(Expression context, Type collection, Type element, IReadOnlyList<Expression> arguments, IAssembler assembler);

        protected Type GetQueryableType(Expression expression)
        {
            if(typeof(IQueryable).IsAssignableFrom(expression.Type))
                return typeof(Queryable);

            if(typeof(IEnumerable).IsAssignableFrom(expression.Type))
                return typeof(Enumerable);

            throw new CompilationException($"'{expression.Type.Name}' is not linq-compatible type.");
        }

        public static IQueryable<T> NonNullCollection<T>(IEnumerable<T> collection)
        {
            IQueryable<T> proxy;

            if (collection == null)
                proxy = new T[] { }.AsQueryable();
            else if (collection is IQueryable<T> queryable)
                proxy = queryable;
            else
                proxy = collection.AsQueryable();

            return proxy;
        }
    }

    public class NonNullCollection<TCollection, T> : IQueryable<T>
        where TCollection : class, IEnumerable<T>
    {
        private IQueryable<T> _collection;

        public Type ElementType => _collection.ElementType;

        public Expression Expression => _collection.Expression;

        public IQueryProvider Provider => _collection.Provider;

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public static explicit operator NonNullCollection<TCollection, T>(TCollection collection)
        {
            var proxy = new NonNullCollection<TCollection, T>();

            if (collection == null)
                proxy._collection = new T[] { }.AsQueryable();
            else if (collection is IQueryable<T> queryable)
                proxy._collection = queryable;
            else
                proxy._collection = collection.AsQueryable();

            return proxy;
        }
    }
}
