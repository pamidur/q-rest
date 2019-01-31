using QRest.Compiler.Standard.Expressions;
using QRest.Compiler.Standard.StringParsing;
using QRest.Core.Contracts;
using QRest.Core.Exceptions;
using QRest.Core.Extensions;
using QRest.Core.Operations;
using QRest.Core.Terms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Assembler
{
    public partial class StandardAssembler : TermVisitor
    {
        protected readonly bool _terminateSelects = true;

        private readonly bool _allowUncompletedQueries;
        private readonly IStringParsingBehavior _stringParsingBehavior;

        public StandardAssembler(bool allowUncompletedQueries, IStringParsingBehavior stringParsingBehavior)
        {
            _allowUncompletedQueries = allowUncompletedQueries;
            _stringParsingBehavior = stringParsingBehavior;
        }

        public (LambdaExpression Lambda, IReadOnlyList<ConstantExpression> Constants)
            Assemble(ITerm sequence, ParameterExpression root, Type expectedType = null)
        {
            var assembled = AssembleTerm(sequence, root, root);

            var expression = assembled.Expression;

            if (expectedType != null)
                expression = Expression.Convert(expression, expectedType);

            var resultLambda = Expression.Lambda(expression, new[] { root }.Concat(assembled.Parameters));
            return (resultLambda, assembled.Constants);
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
                exp = DynamicContainer.CreateReadPropertyIndexer(ctx, p.Name);
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
            if (ctx.Type == typeof(DateTime))
                ctx = Expression.Convert(ctx, typeof(DateTime), TypeConverters.DateTimeToUtc);

            var args = m.Arguments.Select(a => AssembleTerm(a, root, ctx)).ToArray();

            var constants = args.SelectMany(a => a.Constants).ToArray();
            var parameters = args.SelectMany(a => a.Parameters).ToArray();
            var argValues = args.Select(a => a.Expression).ToArray();

            var exp = m.Operation.CreateExpression(root, ctx, argValues, this);

            if (_terminateSelects && !(exp is TerminationExpression))
            {
                var testexp = exp;
                if (testexp is ProxyExpression proxy)
                    testexp = proxy.OriginalExpression;

                if (testexp is MethodCallExpression call && call.Method.Name == nameof(Queryable.Select) && call.Method.DeclaringType == typeof(Queryable))
                    exp = TerminationExpression.Create(exp);
            }

            return (exp, constants, parameters);
        }

        protected override
            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters)
            AssembleName(NameTerm n, ParameterExpression root, Expression ctx)
        {
            return (NamedExpression.Create(ctx, n.Name), null, null);
        }

        protected override (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters)
            AssembleSequence(SequenceTerm s, ParameterExpression root, Expression ctx)
        {
            var result = base.AssembleSequence(s, root, ctx);

            var exp = result.Expression;

            if (!_allowUncompletedQueries)
                exp = TerminationExpression.Create(exp);

            return (exp, result.Constants, result.Parameters);
        }

        protected override
            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters)
            AssembleLambda(LambdaTerm l, ParameterExpression root, Expression ctx)
        {
            if(!ctx.Type.TryGetCollectionElement(out var element))
                throw new TermTreeCompilationException($"Cannot compile lambda '{l.SharedView}' against non-collection type '{ctx.Type}'.");

            var rootarg = Expression.Parameter(element.type, "e");

            var sequence = base.AssembleTerm(l.Term, rootarg, rootarg);

            var resultLambda = Expression.Lambda(sequence.Expression, rootarg);
            return (resultLambda, sequence.Constants, sequence.Parameters);
        } 
    }    
}
