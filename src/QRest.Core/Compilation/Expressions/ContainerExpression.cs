using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Compilation.Expressions
{
    public class ContainerExpression : ProxyExpression
    {
        private ContainerExpression(MemberInitExpression memberInit, IReadOnlyDictionary<string, Expression> properties) : base(memberInit)
        {
            Properties = properties;
        }

        public IReadOnlyDictionary<string, Expression> Properties { get; }

        public static ContainerExpression Create(MemberInitExpression memberInit, IReadOnlyDictionary<string, Expression> properties)
        {
            return new ContainerExpression(memberInit, properties);
        }
    }
}
