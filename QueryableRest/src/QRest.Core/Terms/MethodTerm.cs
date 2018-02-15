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

        public virtual Expression CreateExpression(Expression prev, ParameterExpression root)
        {
            if (!Operation.SupportsCall)
                throw new ExpressionCreationException();

            var args = Arguments.Select(a => a.CreateExpression(prev, root)).ToList();
            var exp = Operation.CreateCallExpression(root, prev, args);

            return Next?.CreateExpression(exp, root) ?? exp;
        }

        public override string ToString()
        {
            var args = string.Join(",", Arguments.Select(a => a.ToString().TrimStart('.')));

            var argsLiteral = args.Length > 0 ? $"({args})" : "";

            return $"-{Operation.GetType().Name.ToLowerInvariant().Replace("operation","")}{argsLiteral}{Next?.ToString()}";
        }
    }
}
