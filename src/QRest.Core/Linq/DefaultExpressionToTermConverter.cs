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
            return new TermExpression(new SequenceTerm(param[0], new MethodTerm(OperationsMap.Where, param.Skip(1).ToArray())));
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return new TermExpression(new ConstantTerm(node.Value));
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return new TermExpression(new MethodTerm(OperationsMap.Root));
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var source = ((TermExpression) Visit(node.Expression)).Term;
            return new TermExpression(new SequenceTerm(source, new PropertyTerm(node.Member.Name)));
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var left = ((TermExpression)Visit(node.Left)).Term;
            var right = ((TermExpression)Visit(node.Right)).Term;

            return new TermExpression(new SequenceTerm(left, new MethodTerm(OperationsMap.Equal, right)));

            return base.VisitBinary(node);
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
            return new TermExpression(new MethodTerm(OperationsMap.Root));
        }
    }
}
