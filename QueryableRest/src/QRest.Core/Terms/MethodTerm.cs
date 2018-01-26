using QRest.Core.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class MethodTerm : ITerm
    {
        public IOperation Operation { get; set; }
        public List<ITerm> Arguments { get; set; } = new List<ITerm>();
        public ITerm Next { get; set; }

        public virtual Expression CreateExpression(Expression prev, ParameterExpression root, QueryContext context)
        {
            var newCtx = context.Derive();

            var args = Arguments.Select(a => a.CreateExpression(prev, root, newCtx)).ToList();
            var exp = Operation.CreateExpression(prev, root, args, newCtx);

            return Next?.CreateExpression(exp, root, context) ?? exp;
        }

        public override string ToString()
        {
            var args = string.Join(",", Arguments.Select(a => a.ToString().TrimStart('.')));

            var argsLiteral = args.Length > 0 ? $"({args})" : "";

            return $"-{Operation.GetType().Name.ToLowerInvariant().Replace("operation","")}{argsLiteral}{Next?.ToString()}";
        }
    }
}
