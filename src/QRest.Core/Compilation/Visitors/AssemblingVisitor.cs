using QRest.Core.Compilation.Containers;
using QRest.Core.Compilation.Expressions;
using QRest.Core.Compilation.TypeConverters;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Compilation.Visitors
{
    public class AssemblingVisitor : TermVisitor<AssemblerState>
    {
        private readonly IContainerFactory _containerFactory;
        private readonly ITypeConverter _typeConverter;
        private readonly bool _terminateSelects;
        private readonly bool _allowUncompletedQueries;

        public AssemblingVisitor(
            IContainerFactory containerFactory,
            ITypeConverter typeConverter,
            bool allowUncompletedQueries,
            bool terminateSelects
            )
        {
            _containerFactory = containerFactory;
            _typeConverter = typeConverter;
            _allowUncompletedQueries = allowUncompletedQueries;
            _terminateSelects = terminateSelects;
        }

        public (LambdaExpression Lambda, IReadOnlyList<(ParameterExpression Param, ConstantExpression Value)> Constants)
            Assemble(ITerm term, ParameterExpression root, Type expectedType = null)
        {
            var assembled = Visit(term, AssemblerState.New(root, new AssemblerServices(_containerFactory, _typeConverter)));

            var expression = assembled.Context;

            if (expectedType != null)
                expression = Expression.Convert(expression, expectedType);

            var resultLambda = Expression.Lambda(expression, new[] { root }.Concat(assembled.Constants.Select(c => c.Param)));
            return (resultLambda, assembled.Constants);
        }

        protected override AssemblerState VisitConstant(ConstantTerm c, in AssemblerState state)
        {
            var paramName = char.ToLowerInvariant(state.Services.GetName(state.Result)[0]).ToString();

            var constant = Expression.Constant(c.Value);
            var param = Expression.Parameter(constant.Type, paramName);

            return state.WithConstant(constant, param).WithResult(param);
        }

        protected override AssemblerState VisitProperty(PropertyTerm p, in AssemblerState state)
        {
            var ctx = state.Context;

            Expression exp;

            if (_containerFactory.IsContainerExpression(ctx))
            {
                exp = _containerFactory.CreateReadProperty(ctx, p.Name);
            }
            else
            {
                exp = Expression.PropertyOrField(ctx, p.Name);
            }

            return state.WithResult(exp);
        }

        protected override AssemblerState VisitMethod(MethodTerm m, in AssemblerState state)
        {
            var methodState = state;

            var argResults = new List<Expression>();
            if (m.Arguments.Count != 0)
            {
                var argCtx = state.Fork();
                foreach (var arg in m.Arguments)
                {
                    var argState = Visit(arg, in argCtx);
                    argResults.Add(argState.Result);
                    methodState = methodState.Merge(argState);
                }
            }

            var exp = m.Operation.CreateExpression(state.Context, argResults, state.Services);

            if (_terminateSelects && !(exp is TerminationExpression))
            {
                var testexp = exp;
                if (testexp is ProxyExpression proxy)
                    testexp = proxy.OriginalExpression;

                if (testexp is MethodCallExpression call && call.Method.Name == nameof(Queryable.Select) && call.Method.DeclaringType == typeof(Queryable))
                    exp = TerminationExpression.Create(exp);
            }

            return methodState.WithResult(exp);
        }

        protected override AssemblerState VisitName(NameTerm n, in AssemblerState state)
        {
            return state.WithResult(NamedExpression.Create(state.Context, n.Name));
        }

        protected override AssemblerState VisitSequence(SequenceTerm s, in AssemblerState state)
        {
            var result = base.VisitSequence(s, state);

            if (!_allowUncompletedQueries)
                result = result.WithResult(TerminationExpression.Create(result.Context));

            return result;
        }

        protected override AssemblerState VisitLambda(LambdaTerm l, in AssemblerState state)
        {
            if (!state.Result.Type.TryGetCollectionElement(out var element))
                throw new CompilationException($"Cannot compile lambda '{l.ViewQuery}' against non-collection type '{state.Context.Type}'.");

            var paramName = char.ToLowerInvariant(state.Services.GetName(state.Result)[0]).ToString();

            var lambdaContext = state.Fork(Expression.Parameter(element.type, paramName));
            var lambdaResult = Visit(l.Term, lambdaContext);
            var lambdaExp = Expression.Lambda(lambdaResult.Context, lambdaResult.Root);

            return state
                .Merge(lambdaResult)
                .WithResult(lambdaExp);
        }

        protected override AssemblerState VisitContext(ContextTerm x, in AssemblerState state)
        {
            if (x.IsRoot)
            {
                return state.Context != state.Root ? state.WithContext(state.Root) : state;
            }
            if (x.IsResult)
            {
                return state.Context != state.Result ? state.WithContext(state.Result) : state;
            }

            throw new NotSupportedException("Named contexts are not supported yet");
        }
    }
}
