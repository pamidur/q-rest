using QRest.Core.Contracts;
using QRest.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class LambdaTerm : MethodTerm
    {
        public LambdaTerm(IOperation operation, IReadOnlyList<ITermSequence> arguments)
        {

        }

        public override Expression CreateExpression(ICompilationContext compiler, Expression prev, ParameterExpression root)
        {
            if (!Operation.SupportsQuery)
                throw new ExpressionCreationException();

            var etype = prev.GetQueryElementType();

            var argsroot = Expression.Parameter(etype, etype.Name.ToLowerInvariant());

            var args = Arguments.Select(a => compiler.Assemble(a, argsroot, argsroot)).ToList();
            var exp = Operation.CreateQueryExpression(prev, argsroot, args);

            return exp;
        }

        protected override string GetView(Func<ITermSequence, string> viewSelector) => $":{ base.GetView(viewSelector).Substring(1)}";
        public override ITerm Clone() => new LambdaTerm { Operation = Operation, Arguments = Arguments.Select(a => a.Clone()).ToList() };
    }
}
