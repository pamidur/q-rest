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
        public List<ITermSequence> Arguments { get; set; } = new List<ITermSequence>();

        public override Expression CreateExpression(ICompilationContext compiler, Expression prev, ParameterExpression root)
        {
            if (!Operation.SupportsCall)
                throw new ExpressionCreationException();

            var args = Arguments.Select(a => compiler.Assemble(a, prev, root)).ToList();
            var exp = Operation.CreateCallExpression(root, prev, args);

            return exp;
        }

        public override string SharedView
        {
            get
            {
                var args = string.Join(",", Arguments.Select(a => a.ToString()));
                var argsLiteral = args.Length > 0 ? $"({args})" : "";
                return $"-{Operation.GetType().Name.ToLowerInvariant().Replace("operation", "")}{argsLiteral}";
            }
        }

        public override ITerm Clone() => new MethodTerm { Operation = Operation, Arguments = Arguments.Select(a => (ITermSequence) a.Clone()).ToList() };
    }
}
