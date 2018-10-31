using QRest.Core.Containers;
using QRest.Core.Contracts;
using QRest.Core.Expressions;
using QRest.Core.Extensions;
using QRest.Core.Terms;
using System;
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

        public (LambdaExpression Lambda, IReadOnlyList<ConstantExpression> Constants) Assemble(SequenceTerm sequence, ParameterExpression root, ParameterExpression ctx = null, bool finalize = true)
        {
            var args = new[] { root };
            var ctxarg = root;
            if (ctx != null && ctx != root)
            {
                ctxarg = ctx;
                args = new[] { root, ctx };
            }

            var assembled = AssembleSequence(sequence, root, ctxarg);


            var resultLambda = Expression.Lambda(assembled.Expression, args.Concat(assembled.Parameters));
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

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleConstant(ConstantTerm c, Expression root, Expression ctx)
        {
            var constant = Expression.Constant(c.Value);
            var param = Expression.Parameter(constant.Type, "v");

            return (param, new[] { constant }, new[] { param });
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleProperty(PropertyTerm p, Expression root, Expression ctx)
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

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleMethod(MethodTerm m, Expression root, Expression ctx)
        {
            var args = m.Arguments.Select(a => AssembleTerm(a, root, ctx)).ToArray();

            var constants = args.SelectMany(a => a.Constants).ToArray();
            var parameters = args.SelectMany(a => a.Parameters).ToArray();
            var argValues = args.Select(a => a.Expression).ToArray();

            return (m.Operation.CreateCallExpression(root, ctx, argValues), constants, parameters);
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleName(NameTerm n, Expression root, Expression ctx)
        {
            return (new NamedExpression(ctx, n.Name), null, null);
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleLambda(LambdaTerm l, Expression root, Expression ctx)
        {
            throw new NotImplementedException();
        }
    }
}
