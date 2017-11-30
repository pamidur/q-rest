using System;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Containers
{
    public class VariableContainer
    {
        public string Name { get; set; }
        public Expression Value { get; set; }
    }
}
