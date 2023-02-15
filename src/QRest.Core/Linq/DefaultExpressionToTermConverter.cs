using QRest.Core.Operations;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Linq
{
    public class DefaultExpressionToTermConverter : IExpressionToTermConverter
    {
        public ITerm Convert<TResult>(Expression expression)
        {
            return ((TermExpression)new ConvertingVisitor().Visit(expression)).Term;
        }
    }

    public class TermExpression : Expression
    {
        public override ExpressionType NodeType => ExpressionType.Extension;
        public TermExpression(ITerm term)
        {
            Term = term;
        }

        public ITerm Term { get; }
    }

    public class ConvertingVisitor : ExpressionVisitor
    {

        public ConvertingVisitor()
        {

        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object != null || !node.Method.IsStatic || node.Method.DeclaringType != typeof(Queryable))
                throw new NotSupportedException("Only Queryable extension method are supported yet");

            var parameters = Visit(node.Arguments).Cast<TermExpression>().Select(te => te.Term).ToArray();

            return node.Method.Name switch
            {
                nameof(Queryable.Where) => MakeGenericMethodTerm(OperationsMap.Where, parameters),
                nameof(Queryable.Select) => MakeGenericMethodTerm(OperationsMap.Map, parameters),
                nameof(Queryable.Skip) => MakeGenericMethodTerm(OperationsMap.Skip, parameters),
                nameof(Queryable.Take) => MakeGenericMethodTerm(OperationsMap.Take, parameters),
                nameof(Queryable.OrderBy) => CreateOrderTerm(parameters, reverse: false),
                nameof(Queryable.OrderByDescending) => CreateOrderTerm(parameters, reverse: true),
                nameof(Queryable.ThenBy) => AppendOrderTerm(parameters, reverse: false),
                nameof(Queryable.ThenByDescending) => AppendOrderTerm(parameters, reverse: true),
                nameof(Queryable.Count) => parameters.Length == 2 ?
                   MakeGenericMethodTerm(OperationsMap.Where, parameters, new MethodTerm(OperationsMap.Count)) :
                   MakeGenericMethodTerm(OperationsMap.Count, parameters),
                nameof(Queryable.First) => parameters.Length == 2 ?
                   MakeGenericMethodTerm(OperationsMap.Where, parameters, new MethodTerm(OperationsMap.First)) :
                   MakeGenericMethodTerm(OperationsMap.First, parameters),
                nameof(Queryable.FirstOrDefault) => parameters.Length == 2 ?
                   MakeGenericMethodTerm(OperationsMap.Where, parameters, new MethodTerm(OperationsMap.First)) :
                   MakeGenericMethodTerm(OperationsMap.First, parameters),

                _ => throw new NotSupportedException($"Method {node.Method.Name} not supported."),
            };
        }

        private Expression CreateOrderTerm(ITerm[] parameters, bool reverse)
        {
            if (reverse)
                parameters[1] = new LambdaTerm(((LambdaTerm)parameters[1]).Term.Chain(new MethodTerm(OperationsMap.Reverse)));
            return new TermExpression(new SequenceTerm(parameters[0], new MethodTerm(OperationsMap.Order, parameters[1..^0])));
        }

        private Expression AppendOrderTerm(ITerm[] parameters, bool reverse)
        {
            var chain = (SequenceTerm)parameters[0];
            var arg = (LambdaTerm)parameters[1];

            var prevs = chain.ToArray()[0..^1];
            var order = (MethodTerm)chain[^1];

            if (reverse)
                arg = new LambdaTerm(arg.Term.Chain(new MethodTerm(OperationsMap.Reverse)));

            var call = new MethodTerm(OperationsMap.Order, order.Arguments.Concat(new[] { arg }).ToArray());
            return new TermExpression(new SequenceTerm(prevs).Chain(call));
        }

        private Expression MakeGenericMethodTerm(IOperation op, ITerm[] parameters, ITerm modifier = null)
        {
            return new TermExpression(new SequenceTerm(parameters[0], new MethodTerm(op, parameters[1..^0]), modifier));
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return new TermExpression(new ConstantTerm(node.Value));
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return new TermExpression(ContextTerm.Root);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var source = ((TermExpression)Visit(node.Expression)).Term;
            return new TermExpression(new SequenceTerm(source, new PropertyTerm(node.Member.Name)));
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var left = ((TermExpression)Visit(node.Left)).Term;
            var right = ((TermExpression)Visit(node.Right)).Term;

            return new TermExpression(SelectBooleanOperation(node, left, right));
        }

        protected virtual ITerm SelectBooleanOperation(BinaryExpression node, ITerm left, ITerm right)
        {
            return node.NodeType switch
            {
                ExpressionType.Equal => new SequenceTerm(left, new MethodTerm(OperationsMap.Equal, right)),
                ExpressionType.NotEqual => new SequenceTerm(left, new MethodTerm(OperationsMap.NotEqual, right)),
                ExpressionType.LessThan => new SequenceTerm(left, new MethodTerm(OperationsMap.LessThan, right)),
                ExpressionType.LessThanOrEqual => new SequenceTerm(left, new MethodTerm(OperationsMap.LessThanOrEqual, right)),
                ExpressionType.GreaterThan => new SequenceTerm(left, new MethodTerm(OperationsMap.GreaterThan, right)),
                ExpressionType.GreaterThanOrEqual => new SequenceTerm(left, new MethodTerm(OperationsMap.GreaterThanOrEqual, right)),
                ExpressionType.OrElse => new MethodTerm(OperationsMap.Or, left, right),
                ExpressionType.AndAlso => new MethodTerm(OperationsMap.And, left, right),
                _ => throw new NotSupportedException($"Operation {node.NodeType} is not supported."),
            };
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var body = ((TermExpression)Visit(node.Body)).Term;
            return new TermExpression(new LambdaTerm(body));
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return base.VisitConditional(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Quote)
                return Visit(node.Operand);

            return base.VisitUnary(node);
        }

        protected override Expression VisitExtension(Expression node)
        {
            return base.VisitExtension(node);
        }

        protected internal virtual Expression VisitRoot()
        {
            return new TermExpression(ContextTerm.Root);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var args = new List<ITerm>();

            for (int i = 0; i < node.Members.Count; i++)
            {
                var member = node.Members[i];
                var arg = ((TermExpression)Visit(node.Arguments[i])).Term;

                args.Add(new SequenceTerm(arg, new NameTerm(member.Name)));
            }

            var nexExp = new MethodTerm(OperationsMap.New, args.ToArray());
            return new TermExpression(nexExp);
        }
    }
}
