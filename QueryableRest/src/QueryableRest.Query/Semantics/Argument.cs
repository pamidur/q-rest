using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QueryableRest.Query.Semantics
{
    public abstract class Argument
    {
        public abstract string[] Monikers { get; }

        public abstract Expression CreateExpression(ParameterExpression parameter);
    }
}