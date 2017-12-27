using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core
{
    public class QueryContext
    {
        private QueryContext()
        {

        }

        public QueryContext(Registry registry)
        {
            Registry = registry;
            NamedExpressions = new NamedExpressionsCollection();
        }

        public NamedExpressionsCollection NamedExpressions { get; private set; }
        public Registry Registry { get; private set; }

        public QueryContext Derive()
        {
            return new QueryContext { Registry = Registry, NamedExpressions = NamedExpressions };
        }
    }

    public class NamedExpressionsCollection
    {
        private readonly Dictionary<Expression, string> _internalCollection = new Dictionary<Expression, string>();
        private readonly NamedExpressionsCollection _parentCollection;

        public NamedExpressionsCollection()
        {

        }
        public NamedExpressionsCollection(NamedExpressionsCollection parentCollection)
        {
            _parentCollection = parentCollection;
        }

        public void AddOrUpdate(string name, Expression expression)
        {
            _internalCollection[expression] = name;
        }

        public bool Contains(Expression expression)
        {
            return _internalCollection.ContainsKey(expression) || (_parentCollection?.Contains(expression) ?? false);
        }

        public bool Contains(string name)
        {
            return _internalCollection.ContainsValue(name) || (_parentCollection?.Contains(name) ?? false);
        }

        public string Get(Expression expression)
        {
            string name = null;
            if (!_internalCollection.TryGetValue(expression, out name))
                name = _parentCollection?.Get(expression);

            return name;
        }
    }
}
