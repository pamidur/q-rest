using QRest.Core;
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
    public partial class StandardAssembler : TermVisitor, IAssemblerContext
    {
        private readonly StandardCompillerOptions _options;

        public StandardAssembler(StandardCompillerOptions options)
        {
            _options = options;
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

            if (_options.TerminateAfterSelect)
            {
                if (exp is MethodCallExpression call && call.Method.Name == "Select" && call.Method.DeclaringType == typeof(Queryable))
                {
                    var element = call.Arguments[0].GetQueryElementType();
                    exp = Terminate(exp, element);
                }
            }

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

            if (_options.TerminateSequence)
            {
                var element = exp.GetQueryElementType();                

                if (element != null)                
                    exp = Terminate(exp, element);                
            }

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

        protected Expression Terminate(Expression exp, Type element)
        {
            var name = GetName(exp) ?? NamedExpression.DefaultQueryResultName;

            exp = NamedExpression.Create(Expression.Call(typeof(Queryable), nameof(Queryable.AsQueryable), new[] { element }
            , Expression.Call(typeof(Enumerable), nameof(Enumerable.ToArray)
            , new[] { element }, exp)), name);
            return exp;
        }

        public virtual (Expression Left, Expression Right) Convert(Expression left, Expression right)
        {
            var leftType = /*left.NodeType == ExpressionType.Lambda ? ((LambdaExpression)left).ReturnType : */left.Type;
            var rightType = /*right.NodeType == ExpressionType.Lambda ? ((LambdaExpression)right).ReturnType :*/ right.Type;

            if (TryConvert(right, leftType, out var newright))
                return (left, newright);
            else if (TryConvert(left, rightType, out var newleft))
                return (newleft, right);
            else
                throw new ExpressionCreationException($"Cannot cast {leftType.Name} and {rightType.Name} either way.");
        }

        public virtual bool TryConvert(Expression expression, Type target, out Expression result)
        {
            if (expression.Type == target)
            {
                result = expression;
                return true;
            }
            else if (target.IsAssignableFrom(expression.Type))
            {
                result = Expression.Convert(expression, target);
                return true;
            }
            else if (expression.Type == typeof(string))
            {
                result = _options.StringParsing.Parse(expression, target);
                return result != null;
            }

            result = null;
            return false;
        }
    }
}
