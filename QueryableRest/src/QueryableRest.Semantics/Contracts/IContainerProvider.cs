using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QRest.Core.Contracts
{
    public interface IContainerProvider
    {
        Expression CreateContainer(IDictionary<string, Expression> properties);
        Expression CreateReadProperty(Expression context, string name);
        bool IsContainerType(Type type);
    }
}
