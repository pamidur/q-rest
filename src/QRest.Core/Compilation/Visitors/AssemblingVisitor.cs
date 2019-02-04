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
    public class AssemblingVisitor : TermVisitor<AssemblerContext>
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

        public (LambdaExpression Lambda, IReadOnlyList<ConstantExpression> Constants)
            Assemble(ITerm term, ParameterExpression root, Type expectedType = null)
        {
            var assembled = Visit(term, new AssemblerContext { Root = root, Context = root, ContainerFactory = _containerFactory, TypeConverter = _typeConverter });

            var expression = assembled.Context;

            if (expectedType != null)
                expression = Expression.Convert(expression, expectedType);

            var resultLambda = Expression.Lambda(expression, new[] { root }.Concat(assembled.Parameters));
            return (resultLambda, assembled.Constants);
        }

        protected override AssemblerContext VisitConstant(ConstantTerm c, AssemblerContext assembler)
        {
            var constant = Expression.Constant(c.Value);
            var param = Expression.Parameter(constant.Type, "v");

            assembler.Constants.Add(constant);
            assembler.Parameters.Add(param);
            assembler.Context = param;

            return assembler;
        }

        protected override AssemblerContext VisitProperty(PropertyTerm p, AssemblerContext assembler)
        {
            var ctx = assembler.Context;

            Expression exp;

            if (_containerFactory.IsContainerExpression(ctx))
            {
                exp = _containerFactory.CreateReadProperty(ctx, p.Name);
            }
            else
            {
                exp = Expression.PropertyOrField(ctx, p.Name);
            }

            assembler.Context = exp;

            return assembler;
        }

        protected override AssemblerContext VisitMethod(MethodTerm m, AssemblerContext assembler)
        {
            var args = new List<Expression>();

            if (m.Arguments.Count != 0)
            {
                var origCtx = assembler.Context;

                foreach (var arg in m.Arguments)
                {
                    assembler = Visit(arg, assembler);
                    args.Add(assembler.Context);
                    assembler.Context = origCtx;
                }
            }

            var exp = m.Operation.CreateExpression(assembler.Root, assembler.Context, args, assembler);

            if (_terminateSelects && !(exp is TerminationExpression))
            {
                var testexp = exp;
                if (testexp is ProxyExpression proxy)
                    testexp = proxy.OriginalExpression;

                if (testexp is MethodCallExpression call && call.Method.Name == nameof(Queryable.Select) && call.Method.DeclaringType == typeof(Queryable))
                    exp = TerminationExpression.Create(exp);
            }

            assembler.Context = exp;

            return assembler;
        }

        protected override AssemblerContext VisitName(NameTerm n, AssemblerContext assembler)
        {
            assembler.Context = NamedExpression.Create(assembler.Context, n.Name);
            return assembler;
        }

        protected override AssemblerContext VisitSequence(SequenceTerm s, AssemblerContext assembler)
        {
            var result = base.VisitSequence(s, assembler);

            if (!_allowUncompletedQueries)
                result.Context = TerminationExpression.Create(result.Context);

            return result;
        }

        protected override AssemblerContext VisitLambda(LambdaTerm l, AssemblerContext assembler)
        {
            if (!assembler.Context.Type.TryGetCollectionElement(out var element))
                throw new CompilationException($"Cannot compile lambda '{l.SharedView}' against non-collection type '{assembler.Context.Type}'.");

            var origroot = assembler.Root;

            assembler.Root = Expression.Parameter(element.type, "e");
            assembler.Context = assembler.Root;

            assembler = Visit(l.Term, assembler);

            assembler.Context = Expression.Lambda(assembler.Context, assembler.Root);
            assembler.Root = origroot;

            return assembler;
        }        
    }
}
