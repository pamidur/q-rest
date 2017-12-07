using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class MethodTerm : ITerm
    {
        public string Method { get; set; }
        public List<ITerm> Arguments { get; set; } = new List<ITerm>();
        public ITerm Next { get; set; }

        public virtual Expression CreateExpression(Expression prev, ParameterExpression root, Registry registry)
        {
            var op = registry.Operations[Method];

            var args = Arguments.Select(a => a.CreateExpression(root, root, registry)).ToList();
            var exp = op.CreateExpression(prev, root, args);

            return Next?.CreateExpression(exp, root, registry) ?? exp;
        }

        public override string ToString()
        {
            var args = string.Join(",", Arguments.Select(a => a.ToString().TrimStart('.')));

            var argsLiteral = args.Length > 0 ? $"({args})" : "";

            return $"-{Method}{argsLiteral}{Next?.ToString()}";
        }
    }
}
