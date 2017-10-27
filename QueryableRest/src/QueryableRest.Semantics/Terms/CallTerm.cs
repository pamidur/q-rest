using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QueryableRest.Semantics.Terms
{
    public class CallTerm : ITerm
    {
        public string Method { get; set; }
        public List<ITerm> Arguments { get; set; } = new List<ITerm>();
        public ITerm Next { get; set; }

        public Expression CreateExpression(Expression context, Registry registry)
        {
            var op = registry.Operations[Method];

            var argroot = op.GetArgumentsRoot(context);

            var args = Arguments.Select(a => a.CreateExpression(argroot, registry)).ToList();

            var exp = op.CreateExpression(context, argroot, args);

            return Next?.CreateExpression(exp, registry) ?? exp;

        }

        public override string ToString()
        {
            return $":{Method}({string.Join(",", Arguments.Select(a => a.ToString().TrimStart('.')))}){Next?.ToString()}";
        }
    }
}
