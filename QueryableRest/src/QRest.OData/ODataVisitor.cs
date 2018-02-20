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

            left.Next = op;
            return left;
        }

        private IOperation GetOperation(ODataGrammarParser.ComparatorContext context)
        {
            switch (context.GetText())
            {
                case "eq":
                    return new EqualOperation();
                default:
                    throw new Exception("Operation not supported");
            }
        }


        public override ITerm VisitIdentifierExpression([NotNull] ODataGrammarParser.IdentifierExpressionContext context)
        {
            return 
                new 
                PropertyTerm { PropertyName = context.GetText() };
        }

        public override ITerm VisitParenExpression([NotNull] ODataGrammarParser.ParenExpressionContext context)
        {
            return Visit(context);
        }

        public override ITerm VisitBoolExpression([NotNull] ODataGrammarParser.BoolExpressionContext context)
        {

            return base.VisitBoolExpression(context);
        }

        public override ITerm VisitComparator([NotNull] ODataGrammarParser.ComparatorContext context)
        {

            return new MethodTerm { Operation = GetOperation(context) };
        }


        public override ITerm VisitBinaryExpression([NotNull] ODataGrammarParser.BinaryExpressionContext context)
        {
            var left = Visit(context.left);
            var right = Visit(context.right);

            var op = VisitBinary(context.op);


            return base.VisitBinaryExpression(context);
        }

        public override ITerm VisitBinary([NotNull] ODataGrammarParser.BinaryContext context)
        {
            var operation = new MethodTerm
            {
                Operation = PickBinaryOperation(context)
            };

            return operation;
        }

        private IOperation PickBinaryOperation(ODataGrammarParser.BinaryContext context)
        {
            var op = ((TerminalNodeImpl)context.children[0]).Symbol.Type;
          switch (op)
            {
                case ODataGrammarParser.AND:
                    return new EveryOperation();
                default:
                    throw new Exception("Operation not supported");
            }
        }

      

      
    }
}
