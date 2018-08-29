using QRest.Core.Contracts;
using QRest.Core.Extensions;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class LambdaTerm : MethodTerm
    {
        public override Expression CreateExpression(ICompilerContext compiler, Expression prev, ParameterExpression root)
        {
            if (!Operation.SupportsQuery)
                throw new ExpressionCreationException();

            var etype = prev.GetQueryElementType();

            var argsroot = Expression.Parameter(etype, etype.Name.ToLowerInvariant());

            var args = Arguments.Select(a => compiler.Compile(a,argsroot,argsroot)).ToList();
            var exp = Operation.CreateQueryExpression(root, prev, argsroot, args);

            return exp;
        }

        public override string DebugView => $":{base.DebugView.Substring(1)}";

        public override ITerm Clone() => new LambdaTerm { Operation = Operation, Next = Next?.Clone(), Arguments = Arguments.Select(a => a.Clone()).ToList() };
    }
}
