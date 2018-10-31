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

        public virtual LambdaExpression Assemble(TermSequence sequence, Type root, Type context)
        {
            var rootparam = Expression.Parameter(root, "r");
            var ctxparam = Expression.Parameter(context, "c");

            var result = AssembleSequence(sequence, rootparam, ctxparam);

            var lambda = Expression.Lambda(Expression.Invoke(result.Lambda, new[] { rootparam, ctxparam }.Concat<Expression>(result.Constants)), rootparam, ctxparam);

            return lambda;
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
            var args = m.Arguments.Select(a => AssembleSequence(a, root, ctx)).ToArray();

            var constants = args.SelectMany(a => a.Constants).ToArray();
            var parameters = args.SelectMany(a => a.Lambda.Parameters.Skip(2)).ToArray();

            var argValues = args.Select(a => Expression.Invoke(a.Lambda, new[] { root, ctx }.Concat(a.Lambda.Parameters.Skip(2)))).ToArray();           


            return ( m.Operation.CreateCallExpression(root, ctx, argValues),constants,parameters);
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleName(NameTerm n, Expression root, Expression ctx)
        {
            return (new NamedExpression(ctx, n.Name), null, null);
        }
    }
}
