using QRest.Core.Containers;
using QRest.Core.Contracts;
using QRest.Core.Expressions;
using QRest.Core.Extensions;
using QRest.Core.Terms;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard
{
    internal class StandardCompilerContext : TermVisitor, ICompilationContext
    {
        private readonly bool _finalize;

        public StandardCompilerContext(bool finalize)
        {
            _finalize = finalize;
        }

        public (LambdaExpression Lambda, IReadOnlyList<ConstantExpression> Constants)
            Assemble(LambdaTerm sequence, ParameterExpression root, bool finalize = true)
        {
            var assembled = AssembleSequence(sequence, root, root);

            var resultLambda = Expression.Lambda(assembled.Expression, new[] { root }.Concat(assembled.Parameters));
            return (resultLambda, assembled.Constants);
        }

        protected virtual Expression Finalize(Expression exp)
        {
            var eType = exp.GetQueryElementType();

            if (eType == null)
                return exp;

            var name = NamedExpression.DefaultQueryResultName;

            if (exp.NodeType == NamedExpression.NamedExpressionType)
                name = ((NamedExpression)exp).Name;

            exp = new NamedExpression(Expression.Call(typeof(Enumerable), nameof(Enumerable.ToArray), new[] { eType }, exp), name);

            return exp;
        }

        protected override
            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters)
            AssembleConstant(ConstantTerm c, ParameterExpression root, Expression ctx)
        {
            var constant = Expression.Constant(c.Value);
            var param = Expression.Parameter(constant.Type, "v");

            return (param, new[] { constant }, new[] { param });
        }

        protected override
            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters)
            AssembleProperty(PropertyTerm p, ParameterExpression root, Expression ctx)
        {
            Expression exp;

            if (DynamicContainer.IsContainerType(ctx.Type))
            {
                exp = DynamicContainer.CreateReadProperty(ctx, p.Name);
            }
            else
            {
                exp = Expression.PropertyOrField(ctx, p.Name);
            }

            return (exp, null, null);
        }

        protected override
            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters)
            AssembleMethod(MethodTerm m, ParameterExpression root, Expression ctx)
        {
            var args = m.Arguments.Select(a => AssembleTerm(a, root, ctx)).ToArray();

            var constants = args.SelectMany(a => a.Constants).ToArray();
            var parameters = args.SelectMany(a => a.Parameters).ToArray();
            var argValues = args.Select(a => a.Expression).ToArray();

            return (m.Operation.CreateExpression(root, ctx, argValues), constants, parameters);
        }

        protected override
            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters)
            AssembleName(NameTerm n, ParameterExpression root, Expression ctx)
        {
            return (new NamedExpression(ctx, n.Name), null, null);
        }

        protected override
            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters)
            AssembleLambda(LambdaTerm l, ParameterExpression root, Expression ctx)
        {
            var rootarg = l.RootProvider.GetRoot(root, ctx);

            var sequence = base.AssembleSequence(l, rootarg, rootarg);

            var resultLambda = Expression.Lambda(sequence.Expression, rootarg);
            return (resultLambda, sequence.Constants, sequence.Parameters);
        }
    }
}
