using QRest.Core;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Expressions
{
    public class ContainerExpression : ProxyExpression
    {
        public static readonly ExpressionType ExpressionNodeType = (ExpressionType)2102;

        private ContainerExpression(MemberInitExpression memberInit, IReadOnlyDictionary<string, Expression> properties) : base(memberInit, ExpressionNodeType)
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
