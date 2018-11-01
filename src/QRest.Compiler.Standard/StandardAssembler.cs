using QRest.Compiler.Standard.Containers;
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
        private readonly bool _finalize;
        private readonly bool _parseStrings;
        private readonly DataStringParser _parser;

        public StandardAssembler(bool finalize, bool parseStrings, DataStringParser parser)
        {
            _finalize = finalize;
            _parseStrings = parseStrings;
            _parser = parser;
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

            return (m.Operation.CreateExpression(root, ctx, argValues, this), constants, parameters);
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
            else if (_parseStrings && expression.Type == typeof(string))
            {
                var parser = _parser.GetParser(target);
                if (parser != null)
                {
                    var value = (string)((ConstantExpression)expression).Value;

                    try
                    {
                        result = Expression.Call(parser.Method, expression);
                        return true;
                    }
                    catch (FormatException)
                    {
                        result = null;
                        return false;
                        //throw new ExpressionCreationException($"Cannot parse '{value}' to {target.Name}");
                    }
                }
            }

            result = null;
            return false;
        }       
    }
}
