using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Contracts
{
    public interface IContainerProvider
    {
        Expression CreateContainer(IReadOnlyDictionary<string, Expression> properties);
        Expression CreateReadProperty(Expression context, string name);
        bool IsContainerType(Type type);
    }
}
