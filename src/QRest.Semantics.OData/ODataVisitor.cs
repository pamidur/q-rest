using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Operations;
using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using QRest.Core.Operations.Query.OrderDirectionOperations;
using QRest.Core.Terms;
using QRest.Semantics.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using static ODataGrammarParser;

namespace QRest.OData
{
    public class ODataVisitor : ODataGrammarBaseVisitor<ITerm>
    {
        public override ITerm VisitParse([NotNull] ParseContext context)
        {
            return new TermSequence { Visit(context.queryOptions()) };
        }

        public override ITerm VisitQueryOptions([NotNull] QueryOptionsContext context)
        {
            var operationLambdas = context.children.OfType<QueryOptionContext>().Select(c => Visit(c)).Cast<LambdaTerm>();

            var sortedLambdas = operationLambdas.Where(c => c != null)
                .OrderBy(c => c.Operation.GetType(), new ODataOperationOrder()).ToList();

            return BuildTerms(sortedLambdas);
        }

        private ITerm BuildTerms(List<LambdaTerm> sortedLambdas)
        {
            var seq = new TermSequence();

            if (sortedLambdas.Any())
            {
                if (sortedLambdas.First().Operation is WhereOperation)
                {
                    seq.Add(sortedLambdas.First());
                    sortedLambdas = sortedLambdas.Skip(1).ToList();
                }
            }

            var selectTerm = new MethodTerm { Operation = new SelectOperation(), Arguments = new List<ITermSequence>() };
            seq.Add(selectTerm);

            var countTerm = sortedLambdas.Where(c => c.Operation is CountOperation).FirstOrDefault();
            if (countTerm != null) selectTerm.Arguments.Add(new TermSequence { countTerm , new NameTerm { Name = "@odata.count"} });

            selectTerm.Arguments.Add(BuildSelectArgs(sortedLambdas));

            return seq;
        }

        private ITermSequence BuildSelectArgs(List<LambdaTerm> sortedLambdas)
        {
            var seq = new TermSequence();

            foreach (var lambda in sortedLambdas.Where(c => !(c.Operation is CountOperation)))
                seq.Add(lambda);

            if (seq.IsEmpty)
                seq.Add(new LambdaTerm { Operation = new SelectOperation() });

            seq.Add(new NameTerm { Name = "value" });

            return seq;
        }

        private static QueryOptionContext GetContext<T>(IEnumerable<QueryOptionContext> opts)
        {
            return opts.Where(c => c.children.Any(x => x is T)).FirstOrDefault();
        }


        public override ITerm VisitFilter([NotNull] ODataGrammarParser.FilterContext context)
        {
            var termFilter = new LambdaTerm
            {
                Operation = new WhereOperation(),
                Arguments = new List<ITermSequence>()
            };

            termFilter.Arguments.Add(Visit(context.filterexpr).AsSequence());
            return termFilter;
        }

        public override ITerm VisitSelect([NotNull] SelectContext context)
        {
            var selectArgs = context.children.OfType<SelectItemContext>().Select(c => Visit(c)).ToList();
            var lambda = new LambdaTerm { Operation = new SelectOperation { }, Arguments = selectArgs.Select(p=>p.AsSequence()).ToList() };
            return lambda;
        }

        public override ITerm VisitSelectItem([NotNull] SelectItemContext context)
        {
            return new TermSequence {
                new MethodTerm { Operation = new ItOperation() },
                new PropertyTerm { PropertyName = context.GetText() }
            };
        }


        public override ITerm VisitNotExpression([NotNull] NotExpressionContext context)
        {
            return new TermSequence {
                Visit(context.expression()),
                new MethodTerm{ Operation = new NotOperation() }
            };
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
            var left = Visit(context.left).AsSequence();
            var right = Visit(context.right).AsSequence();

            var op = Visit(context.op);
            (op as MethodTerm).Arguments = new List<ITermSequence> { right };

            left.Add(op);

            return left;
        }

        public override ITerm VisitIdentifierExpression([NotNull] ODataGrammarParser.IdentifierExpressionContext context)
        {
            return new TermSequence
            {
                new MethodTerm{ Operation = new ItOperation() },
                new PropertyTerm { PropertyName = context.GetText() }
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
            var left = Visit(context.left).AsSequence();
            var right = Visit(context.right).AsSequence();

            var op = VisitBinary(context.op);
            ((MethodTerm)op).Arguments = new List<ITermSequence> { left, right };

            return new TermSequence { op };
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
            var funcRoot = parameters.First().AsSequence();
            var func = GetFuncTerm(context.func.Text);
            func.Arguments = parameters.Skip(1).Select(p=>p.AsSequence()).ToList();
            funcRoot.Add(func);

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

        public override ITerm VisitOrderby([NotNull] OrderbyContext context)
        {
            var args = context.children.OfType<OrderbyItemContext>().Select(c => Visit(c)).Select(p=>p.AsSequence()).ToList();

            return new LambdaTerm { Operation = new OrderOperation(), Arguments = args };
        }

        public override ITerm VisitOrderbyItem([NotNull] OrderbyItemContext context)
        {
            ITerm order = context.ChildCount > 1 ?
                Visit(context.children[1])
                : new MethodTerm { Operation = new AscendingOperation() };


            return new TermSequence {
                new MethodTerm  {   Operation = new ItOperation() },
                new PropertyTerm    {   PropertyName = context.children[0].GetText() },
                order
            };

        }

        public override ITerm VisitOrder([NotNull] OrderContext context)
        {
            var method = new MethodTerm();
            switch (context.GetText())
            {
                case "asc":
                    method.Operation = new AscendingOperation();
                    break;
                case "desc":
                    method.Operation = new DescendingOperation();
                    break;
                default:
                    throw new Exception($"Order direction {context.GetText()} is unknown");

            }
            return method;
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
