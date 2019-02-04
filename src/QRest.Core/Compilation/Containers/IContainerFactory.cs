using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Compilation.Containers
{
    public interface IContainerFactory
    {
        Expression CreateContainer(IReadOnlyDictionary<string, Expression> properties);
        Expression CreateReadProperty(Expression context, string name);
        bool IsContainerExpression(Expression expression);
    }
}
