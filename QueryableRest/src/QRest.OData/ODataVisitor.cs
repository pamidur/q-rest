using QRest.Core.Terms;
using System;
using Antlr4.Runtime.Misc;
using QRest.Core.Operations.Query;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations;
using Antlr4.Runtime.Tree;

namespace QRest.OData
{
    public class ODataVisitor : ODataGrammarBaseVisitor<ITerm>
    {
        public override ITerm VisitFilter([NotNull] ODataGrammarParser.FilterContext context)
        {
            var termFilter = new LambdaTerm
            {
                Operation = new WhereOperation(),
                Arguments = new System.Collections.Generic.List<ITerm>()
            };

            termFilter.Arguments.Add(Visit(context.filterexpr));
            return termFilter;
        }

        public override ITerm VisitStringExpression([NotNull] ODataGrammarParser.StringExpressionContext context)
        {
            return new ConstantTerm { Value = context.GetText().Trim('\'') };
        }

        public override ITerm VisitComparatorExpression([NotNull] ODataGrammarParser.ComparatorExpressionContext context)
        {
            var left = Visit(context.left);
            var right = Visit(context.right);
            var op = Visit(context.op);

            (op as MethodTerm).Arguments = new System.Collections.Generic.List<ITerm> { right };

            if (left.Next == null) left.Next = op; // for constants
            else left.Next.Next = op; // for ItOperation
            return left;
        }

        private IOperation GetOperation(ODataGrammarParser.ComparatorContext context)
        {
            var op = ((TerminalNodeImpl)context.children[0]).Symbol.Type;
            switch (op)
            {
                case ODataGrammarParser.EQ:
                    return new EqualOperation();
                default:
                    throw new Exception("Operation not supported");
            }
        }

        public override ITerm VisitIdentifierExpression([NotNull] ODataGrammarParser.IdentifierExpressionContext context)
        {
            return new MethodTerm
            {
                Operation = new ItOperation(),
                Next = new PropertyTerm { PropertyName = context.GetText() }
            };
        }

        public override ITerm VisitParenExpression([NotNull] ODataGrammarParser.ParenExpressionContext context)
        {
            return Visit(context.children[1]);
        }

        public override ITerm VisitBoolExpression([NotNull] ODataGrammarParser.BoolExpressionContext context)
        {
            return base.VisitBoolExpression(context);
        }

        public override ITerm VisitBinaryExpression([NotNull] ODataGrammarParser.BinaryExpressionContext context)
        {
            var left = Visit(context.left);
            var right = Visit(context.right);

            var op = VisitBinary(context.op);
            ((MethodTerm)op).Arguments = new System.Collections.Generic.List<ITerm> { left, right };

            return op;
        }

        public override ITerm VisitTerminal(ITerminalNode node)
        {
            OperationBase operation;
            switch (node.Symbol.Type)
            {
                case ODataGrammarParser.AND:
                    operation = new EveryOperation();
                    break;
                case ODataGrammarParser.OR:
                    operation = new OneOfOperation();
                    break;

                case ODataGrammarParser.EQ:
                    operation = new EqualOperation();
                    break;
                default:
                    throw new Exception("Operation not supported");
            }
            return new MethodTerm { Operation = operation };
        }


    }
}
