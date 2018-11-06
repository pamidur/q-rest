using QRest.Compiler.Standard.Expressions;
using QRest.Core.Expressions;
using QRest.Core.Extensions;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Assembler
{
    public partial class StandardAssembler : TermVisitor
    {
        protected readonly bool _terminateSelects = true;

        private readonly IAssemblerOptions _options;

        public StandardAssembler(IAssemblerOptions options)
        {
            _options = options;
        }

        public (LambdaExpression Lambda, IReadOnlyList<ConstantExpression> Constants)
            Assemble(LambdaTerm sequence, ParameterExpression root, Type expectedType = null)
        {
            var assembled = AssembleSequence(sequence, root, root);

            var expression = assembled.Expression;

            if (expectedType != null)
                expression = Expression.Convert(expression, expectedType);

            var resultLambda = Expression.Lambda(expression, new[] { root }.Concat(assembled.Parameters));
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

            exp = NamedExpression.Create(Expression.Call(typeof(Enumerable), nameof(Enumerable.ToArray), new[] { eType }, exp), name);

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

            var exp = m.Operation.CreateExpression(root, ctx, argValues, this);

            if (_terminateSelects && exp is MethodCallExpression call && call.Method.Name == "Select" && call.Method.DeclaringType == typeof(Queryable))            
                exp = TerminationExpression.Create(exp);            

            return (exp, constants, parameters);
        }

        protected override
            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters)
            AssembleName(NameTerm n, ParameterExpression root, Expression ctx)
        {
            return (NamedExpression.Create(ctx, n.Name), null, null);
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleSequence(SequenceTerm s, ParameterExpression root, Expression ctx)
        {
            var result = base.AssembleSequence(s, root, ctx);

            var exp = result.Expression;

            if (!_options.AllowUncompletedQueries)
                exp = TerminationExpression.Create(exp);

            return (exp, result.Constants, result.Parameters);
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
