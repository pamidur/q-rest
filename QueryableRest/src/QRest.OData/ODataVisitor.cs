using QRest.Core.Terms;
using System;
using Antlr4.Runtime.Misc;
using QRest.Core.Operations.Query;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations;
using Antlr4.Runtime.Tree;
using System.Linq;
using static ODataGrammarParser;
using QRest.Core;
using QRest.Core.Operations.Aggregations;
using QRest.Semantics.OData;

namespace QRest.OData
{
    public class ODataVisitor : ODataGrammarBaseVisitor<ITerm>
    {
        public override ITerm VisitParse([NotNull] ParseContext context)
        {
            return Visit(context.queryOptions());
        }

        public override ITerm VisitQueryOptions([NotNull] QueryOptionsContext context)
        {
            var operationLambdas = context.children.OfType<QueryOptionContext>().Select(c => Visit(c)).Cast<LambdaTerm>();

            var sortedLambdas = operationLambdas.Where(c => c != null)
                .OrderBy(c => c.Operation.GetType(), new ODataOperationOrder()).ToList();

            var firstOperation = sortedLambdas.First();
            var prev = firstOperation;
            foreach (var op in sortedLambdas.Skip(1))
            {
                prev.Next = op;
                prev = op;
            }
            return firstOperation;
        }

        private static QueryOptionContext GetContext<T>(System.Collections.Generic.IEnumerable<QueryOptionContext> opts)
        {
            return opts.Where(c => c.children.Any(x => x is T)).FirstOrDefault();
        }


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

        public override ITerm VisitNotExpression([NotNull] NotExpressionContext context)
        {
            var exp = Visit(context.expression());
            var tail = exp.GetLatestCall();
            tail.Next = new MethodTerm
            {
                Operation = new NotOperation(),
            };

            return exp;
        }

        public override ITerm VisitStringExpression([NotNull] ODataGrammarParser.StringExpressionContext context)
        {
            return new ConstantTerm { Value = context.GetText().Trim('\'') };
        }

        public override ITerm VisitDecimalExpression([NotNull] ODataGrammarParser.DecimalExpressionContext context)
        {
            return new ConstantTerm { Value = decimal.Parse(context.GetText()) };
        }

        public override ITerm VisitIntExpression([NotNull] ODataGrammarParser.IntExpressionContext context)
        {
            return new ConstantTerm { Value = int.Parse(context.GetText()) };
        }

        public override ITerm VisitComparatorExpression([NotNull] ODataGrammarParser.ComparatorExpressionContext context)
        {
            var left = Visit(context.left);
            var right = Visit(context.right);
            var op = Visit(context.op);

            (op as MethodTerm).Arguments = new System.Collections.Generic.List<ITerm> { right };
            left.GetLatestCall().Next = op;
            return left;
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
            return new ConstantTerm { Value = bool.Parse(context.GetText()) };
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

                case ODataGrammarParser.NE:
                    operation = new NotEqualOperation();
                    break;

                case ODataGrammarParser.LT:
                    operation = new LessThanOperation();
                    break;
                case ODataGrammarParser.GT:
                    operation = new GreaterThanOperation();
                    break;
                case ODataGrammarParser.LE:
                    operation = new LessThanOrEqualOperation();
                    break;
                case ODataGrammarParser.GE:
                    operation = new GreaterThanOrEqualOperation();
                    break;
                case ODataGrammarParser.Eof:
                    return base.VisitTerminal(node);
                default:
                    throw new Exception("Operation not supported");
            }
            return new MethodTerm { Operation = operation };
        }

        public override ITerm VisitFuncCallExpression([NotNull] FuncCallExpressionContext context)
        {
            var parameters = context.functionParams().children.Where((c, i) => i % 2 == 0).Select(c => Visit(c));
            if (!parameters.Any()) throw new ArgumentException("Need more arguments!");
            var funcRoot = parameters.First();
            var func = GetFuncTerm(context.func.Text);
            func.Arguments = parameters.Skip(1).ToList();
            funcRoot.GetLatestCall().Next = func;

            return funcRoot;
        }

        public override ITerm VisitFunctionParams([NotNull] FunctionParamsContext context)
        {
            return base.VisitFunctionParams(context);
        }


        public override ITerm VisitCount([NotNull] CountContext context)
        {

            if (context.decexpr.GetText().Equals("true", StringComparison.OrdinalIgnoreCase))
                return new LambdaTerm
                {
                    Operation = new CountOperation()
                };
            else return null;
        }

        private MethodTerm GetFuncTerm(string funcName)
        {
            OperationBase operation;

            switch (funcName)
            {
                //case "IsEmpty":
                //    return typeof(string).GetMethod("IsNullOrWhiteSpace", new Type[] { typeof(string) });
                case "contains":
                    operation = new ContainsOperation();
                    break;
                default:
                    throw new Exception($"Function {funcName} not found");
            }

            return new MethodTerm { Operation = operation };
        }

    }
}
