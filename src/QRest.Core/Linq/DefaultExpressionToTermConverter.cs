using QRest.Core.Operations;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
            var param = Visit(node.Arguments).Cast<TermExpression>().Select(te => te.Term).ToArray();
            return new TermExpression(new SequenceTerm(param[0], new MethodTerm(SelectQueryOperation(node), param.Skip(1).ToArray())));
        }

        protected virtual IOperation SelectQueryOperation(MethodCallExpression node)
        {
            if (node.Object != null || !node.Method.IsStatic || node.Method.DeclaringType != typeof(Queryable))
                throw new NotSupportedException("Only Queryable extension method are supported yet");

            switch (node.Method.Name)
            {
                case nameof(Queryable.Where): return OperationsMap.Where;
                case nameof(Queryable.Select): return OperationsMap.Map;
                case nameof(Queryable.Skip): return OperationsMap.Skip;
                case nameof(Queryable.Take): return OperationsMap.Take;
                case nameof(Queryable.OrderBy): return OperationsMap.Order;
                case nameof(Queryable.OrderByDescending): return OperationsMap.Order;
                default: throw new NotSupportedException($"Method {node.Method.Name} not supported.");
            }
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
            switch (node.NodeType)
            {
                case ExpressionType.Equal: return new SequenceTerm(left, new MethodTerm(OperationsMap.Equal, right));
                case ExpressionType.NotEqual: return new SequenceTerm(left, new MethodTerm(OperationsMap.NotEqual, right));
                case ExpressionType.LessThan: return new SequenceTerm(left, new MethodTerm(OperationsMap.LessThan, right));
                case ExpressionType.LessThanOrEqual: return new SequenceTerm(left, new MethodTerm(OperationsMap.LessThanOrEqual, right));
                case ExpressionType.GreaterThan: return new SequenceTerm(left, new MethodTerm(OperationsMap.GreaterThan, right));
                case ExpressionType.GreaterThanOrEqual: return new SequenceTerm(left, new MethodTerm(OperationsMap.GreaterThanOrEqual, right));
                case ExpressionType.OrElse: return new MethodTerm(OperationsMap.OneOf, left, right);
                case ExpressionType.AndAlso: return new MethodTerm(OperationsMap.Every, left, right);
                default: throw new NotSupportedException($"Operation {node.NodeType} is not supported.");
            }
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
    }
}
