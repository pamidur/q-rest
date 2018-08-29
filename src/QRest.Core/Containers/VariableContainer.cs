using System;
using System.Linq.Expressions;

namespace QRest.Core.Containers
{
    public class VariableContainer
    {
        public string Name { get; set; }
        public Expression Value { get; set; }
    }
}
