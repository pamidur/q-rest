using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Operations;
using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using QRest.Core.Operations.Selectors;
using QRest.Core.Terms;
using QRest.Semantics.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using static ODataGrammarParser;

namespace QRest.OData
{
    public class ODataVisitor : ODataGrammarBaseVisitor<SequenceTerm>
    {
        private string _currentContext = string.Empty;

        public override SequenceTerm VisitParse([NotNull] ParseContext context)
        {
            if (context.children.Count < 2)
                return new ODataTermContainer { Data = new MethodTerm(OperationsMap.Context) };

            return Visit(context.queryOptions());
        }

        public override SequenceTerm VisitQueryOptions([NotNull] QueryOptionsContext context)
        {
            var operationLambdas = context.children.OfType<QueryOptionContext>().Select(c => Visit(c));

            var sortedLambdas = operationLambdas.Where(c => c != null && !c.IsEmpty)
                .OrderBy(c => ((MethodTerm)c.Root).Operation.GetType(), new ODataOperationOrder()).ToList();

            return BuildTerms(sortedLambdas);
        }

        private SequenceTerm BuildTerms(List<SequenceTerm> sortedLambdas)
        {
            ITerm dataOut = null;
            ITerm countOut = null;

            if (sortedLambdas.Any())
            {
                var filter = sortedLambdas.FirstOrDefault(l => l.Root is MethodTerm method && method.Operation is WhereOperation);
                if (filter != null)
                {
                    dataOut = filter.Root;
                    sortedLambdas = sortedLambdas.Except(new[] { filter }).ToList();
                }

                var count = sortedLambdas.FirstOrDefault(l => l.Root is MethodTerm method && method.Operation is CountOperation);
                if (count != null)
                {
                    countOut = new SequenceTerm(dataOut, count.Root);
                    sortedLambdas = sortedLambdas.Except(new[] { count }).ToList();
                }

                if (sortedLambdas.Any())
                    dataOut = new SequenceTerm(new[] { dataOut }.Concat(sortedLambdas).ToArray());
            }

            return new ODataTermContainer() { Data = dataOut ?? new MethodTerm(OperationsMap.Context), Count = countOut };
        }

        private static QueryOptionContext GetContext<T>(IEnumerable<QueryOptionContext> opts)
        {
            return opts.Where(c => c.children.Any(x => x is T)).FirstOrDefault();
        }

        public override SequenceTerm VisitFilter([NotNull] ODataGrammarParser.FilterContext context)
        {
            var filterExpression = Visit(context.filterexpr);

            var termFilter = new MethodTerm(new WhereOperation(), new[] { new LambdaTerm(filterExpression) });
            return termFilter.AsSequence();
        }

        public override SequenceTerm VisitSelect([NotNull] SelectContext context)
        {
            var selectArgs = context.children.OfType<SelectItemContext>().Select(c => Visit(c)).ToList();
            var select = new MethodTerm(new EachOperation(), new[] { new LambdaTerm(new MethodTerm(new MapOperation(), selectArgs.ToArray())) });
            return select.AsSequence();
        }

        public override SequenceTerm VisitSelectItem([NotNull] SelectItemContext context)
        {
            return new SequenceTerm(
                new MethodTerm(new RootOperation()),
                new PropertyTerm(context.GetText())
            );
        }


        public override SequenceTerm VisitNotExpression([NotNull] NotExpressionContext context)
        {
            return new SequenceTerm(
                Visit(context.expression()),
                new MethodTerm(new NotOperation())
            );
        }

        public override SequenceTerm VisitStringExpression([NotNull] ODataGrammarParser.StringExpressionContext context)
        {
            return new ConstantTerm(context.GetText().Trim('\'')).AsSequence();
        }

        public override SequenceTerm VisitDecimalExpression([NotNull] ODataGrammarParser.DecimalExpressionContext context)
        {
            return new ConstantTerm(decimal.Parse(context.GetText())).AsSequence();
        }

        public override SequenceTerm VisitIntExpression([NotNull] ODataGrammarParser.IntExpressionContext context)
        {
            return new ConstantTerm(int.Parse(context.GetText())).AsSequence();
        }

        public override SequenceTerm VisitComparatorExpression([NotNull] ODataGrammarParser.ComparatorExpressionContext context)
        {
            var left = Visit(context.left);
            var right = Visit(context.right);

            var op = (MethodTerm)Visit(context.op).Root;

            return new SequenceTerm(left.Concat(new[] { new MethodTerm(op.Operation, new[] { right }) }).ToArray());
        }

        public override SequenceTerm VisitIdentifierExpression([NotNull] ODataGrammarParser.IdentifierExpressionContext context)
        {
            if (string.IsNullOrEmpty(_currentContext) || (context.prefix?.Text?.Equals(_currentContext) ?? false))
                return new SequenceTerm
                (
                    new MethodTerm(new RootOperation()),
                    new PropertyTerm(context.val.Text)
                );
            else
                return new SequenceTerm
                (
                    new MethodTerm(new RootOperation())
                );
        }

        public override SequenceTerm VisitParenExpression([NotNull] ODataGrammarParser.ParenExpressionContext context)
        {
            return Visit(context.children[1]);
        }

        public override SequenceTerm VisitBoolExpression([NotNull] ODataGrammarParser.BoolExpressionContext context)
        {
            return new ConstantTerm(bool.Parse(context.GetText())).AsSequence();
        }

        public override SequenceTerm VisitBinaryExpression([NotNull] ODataGrammarParser.BinaryExpressionContext context)
        {
            var left = Visit(context.left);
            var right = Visit(context.right);

            var op = (MethodTerm)VisitBinary(context.op).Root;
            return new MethodTerm(op.Operation, left, right).AsSequence();
        }

        public override SequenceTerm VisitTerminal(ITerminalNode node)
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
            return new SequenceTerm(new MethodTerm(operation));
        }

        public override SequenceTerm VisitFuncCallExpression([NotNull] FuncCallExpressionContext context)
        {
            var parameters = context.functionParams().children.Where((c, i) => i % 2 == 0).Select(c => Visit(c));
            if (!parameters.Any()) throw new ArgumentException("Need more arguments!");
            var funcRoot = parameters.First();
            var func = GetFuncTerm(context.func.Text);

            return new SequenceTerm(funcRoot.Concat(new[] { new MethodTerm(func.Operation, parameters.Skip(1).ToArray()) }).ToArray());
        }

        public override SequenceTerm VisitFunctionParams([NotNull] FunctionParamsContext context)
        {
            return base.VisitFunctionParams(context);
        }


        public override SequenceTerm VisitCount([NotNull] CountContext context)
        {

            if (context.decexpr.GetText().Equals("true", StringComparison.OrdinalIgnoreCase))
                return new MethodTerm(new CountOperation()).AsSequence();
            else return new SequenceTerm();
        }

        public override SequenceTerm VisitOrderby([NotNull] OrderbyContext context)
        {
            var args = context.children.OfType<OrderbyItemContext>().Select(c => Visit(c)).ToList();

            return new MethodTerm(new OrderOperation(), args.Select(a => new LambdaTerm(a)).ToArray()).AsSequence();
        }

        public override SequenceTerm VisitOrderbyItem([NotNull] OrderbyItemContext context)
        {
            SequenceTerm order = context.ChildCount > 1 ?
                Visit(context.children[1])
                : new MethodTerm(new ContextOperation()).AsSequence();


            return new SequenceTerm(
                new MethodTerm(new RootOperation()),
                new PropertyTerm(context.children[0].GetText()),
                order
            );
        }

        public override SequenceTerm VisitOrder([NotNull] OrderContext context)
        {
            MethodTerm method;
            switch (context.GetText())
            {
                case "asc":
                    method = new MethodTerm(new ContextOperation());
                    break;
                case "desc":
                    method = new MethodTerm(new ReverseOrderOperation());
                    break;
                default:
                    throw new Exception($"Order direction {context.GetText()} is unknown");

            }
            return method.AsSequence();
        }

        public override SequenceTerm VisitSkip([NotNull] SkipContext context)
        {
            if (int.TryParse(context.INT().GetText(), out var value) && value > 0)
                return new MethodTerm(new SkipOperation(), new[] { new ConstantTerm(value).AsSequence() }).AsSequence();
            else return new SequenceTerm();
        }

        public override SequenceTerm VisitTop([NotNull] TopContext context)
        {
            if (int.TryParse(context.INT().GetText(), out var value) && value > 0)
                return new MethodTerm(new TakeOperation(), new[] { new ConstantTerm(value).AsSequence() }).AsSequence();
            else return new SequenceTerm();
        }

        public override SequenceTerm VisitDatetimeExpression([NotNull] DatetimeExpressionContext context)
        {
            var dt = new ConstantTerm(DateTimeOffset.Parse(context.GetText()));
            return dt.AsSequence();


        }

        public override SequenceTerm VisitLambda([NotNull] LambdaContext context)
        {
            var prevContext = _currentContext;

            var field = new SequenceTerm
                (
                    new MethodTerm(new RootOperation()),
                    new PropertyTerm(context.fld.Text)
                );

            _currentContext = context.lid.Text;
            var result = Visit(context.laExpr);
            _currentContext = prevContext;

            var l = new LambdaTerm(result);

            var lname = context.lambdaName().GetText();

            var laname = new SequenceTerm(field,
                  new MethodTerm(OperationsMap.Any,new SequenceTerm[] {
                  new SequenceTerm(l) })
                );

            return laname;
        }


      

        private MethodTerm GetFuncTerm(string funcName)
        {
            OperationBase operation;

            switch (funcName)
            {
                //case "IsEmpty":
                //    return typeof(string).GetMethod("IsNullOrWhiteSpace", new Type[] { typeof(string) });
                case "contains":
                    operation = new HasOperation();
                    break;
                default:
                    throw new Exception($"Function {funcName} not found");
            }

            return new MethodTerm(operation);
        }

    }
}
