using QRest.Core.Terms;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Assembler
{
    public class ConstantsCollector : TermVisitor
    {
        private static readonly ParameterExpression _fakeroot = Expression.Parameter(typeof(object));

        public IReadOnlyList<ConstantExpression> Collect(RootTerm lambda)
        {
            var assembled = AssembleTerm(lambda, _fakeroot, _fakeroot);
            return assembled.Constants;
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleConstant(ConstantTerm c, ParameterExpression root, Expression ctx)
        {
            var constant = Expression.Constant(c.Value);
            return (ctx, new[] { constant }, new ParameterExpression[] { });
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleLambda(LambdaTerm l, ParameterExpression root, Expression ctx)
        {
            return AssembleSequence(l, root, ctx);
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleMethod(MethodTerm m, ParameterExpression root, Expression ctx)
        {
            var args = m.Arguments.Select(a => AssembleTerm(a, root, ctx)).ToArray();

            return (ctx, args.SelectMany(a => a.Constants).ToArray(), new ParameterExpression[] { });
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleName(NameTerm n, ParameterExpression root, Expression ctx)
        {
            return (ctx, new ConstantExpression[] { }, new ParameterExpression[] { });
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleProperty(PropertyTerm p, ParameterExpression root, Expression ctx)
        {
            return (ctx, new ConstantExpression[] { }, new ParameterExpression[] { });
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleRoot(RootTerm r, ParameterExpression root, Expression ctx)
        {
            return AssembleSequence(r, root, ctx);
        }
    }
}
