using QRest.Core.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class MethodTerm : TermBase
    {
        public IOperation Operation { get; set; }
        public List<ITerm> Arguments { get; set; } = new List<ITerm>();

        protected override Expression CreateExpression(Expression prev, ParameterExpression root)
        {
            if (!Operation.SupportsCall)
                throw new ExpressionCreationException();

            var args = Arguments.Select(a => a.CreateExpressionChain(prev, root)).ToList();
            var exp = Operation.CreateCallExpression(root, prev, args);

            return exp;
        }

        protected override string Debug
        {
            get
            {
                var args = string.Join(",", Arguments.Select(a => a.ToString()));
                var argsLiteral = args.Length > 0 ? $"({args})" : "";
                return $"-{Operation.GetType().Name.ToLowerInvariant().Replace("operation", "")}{argsLiteral}";
            }
        }
    }
}
