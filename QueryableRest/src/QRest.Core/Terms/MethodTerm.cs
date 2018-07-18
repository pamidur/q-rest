using QRest.Core.Contracts;
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

        public override Expression CreateExpression(ICompilerContext compiler, Expression prev, ParameterExpression root)
        {
            if (!Operation.SupportsCall)
                throw new ExpressionCreationException();

            var args = Arguments.Select(a => compiler.Compile(a, prev, root)).ToList();
            var exp = Operation.CreateCallExpression(root, prev, args);

            return exp;
        }

        public override string DebugView
        {
            get
            {
                var args = string.Join(",", Arguments.Select(a => a.ToString().TrimStart('.')));
                var argsLiteral = args.Length > 0 ? $"({args})" : "";
                return $"-{Operation.GetType().Name.ToLowerInvariant().Replace("operation", "")}{argsLiteral}";
            }
        }

        public override ITerm Clone() => new MethodTerm { Operation = Operation, Next = Next?.Clone(), Arguments = Arguments.Select(a => a.Clone()).ToList() };
    }
}
