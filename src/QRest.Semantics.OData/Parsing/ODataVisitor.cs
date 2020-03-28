using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using QRest.Core.Operations;
using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Query;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using static ODataGrammarParser;

namespace QRest.Semantics.OData.Parsing
{
    public class ODataVisitor : ODataGrammarBaseVisitor<ITerm>
    {
        private string _currentContext = string.Empty;
        private readonly ODataOperationMap _operations;

        public ODataVisitor(ODataOperationMap operations)
        {
            _operations = operations;
        }

        public override ITerm VisitParse([NotNull] ParseContext context)
        {
            if (context.children.Count < 2)
                return new ODataTermContainer { Data = ContextTerm.Root };

            return Visit(context.queryOptions());
        }

        public override ITerm VisitQueryOptions([NotNull] QueryOptionsContext context)
        {
            var operationLambdas = context.children.OfType<QueryOptionContext>().Select(c => Visit(c));

            var sortedLambdas = operationLambdas.Where(c => c != null)
                .OrderBy(c => ((MethodTerm)c).Operation, new ODataOperationOrder(_operations)).ToList();

            return BuildTerms(sortedLambdas);
        }

        private ITerm BuildTerms(List<ITerm> sortedLambdas)
        {
            ITerm dataOut = ContextTerm.Root;
            ITerm countOut = null;

            if (sortedLambdas.Any())
            {
                var filter = sortedLambdas.FirstOrDefault(l => l is MethodTerm method && method.Operation is WhereOperation);
                if (filter != null)
                {
                    dataOut = filter;
                    sortedLambdas = sortedLambdas.Except(new[] { filter }).ToList();
                }

                var count = sortedLambdas.FirstOrDefault(l => l is MethodTerm method && method.Operation is CountOperation);
                if (count != null)
                {
                    countOut = new SequenceTerm(dataOut, count);
                    sortedLambdas = sortedLambdas.Except(new[] { count }).ToList();
                }

                if (sortedLambdas.Any())
                    dataOut = new SequenceTerm(new[] { dataOut }.Concat(sortedLambdas).ToArray());
            }

            return new ODataTermContainer() { Data = dataOut ?? ContextTerm.Root, Count = countOut };
        }

        private static QueryOptionContext GetContext<T>(IEnumerable<QueryOptionContext> opts)
        {
            return opts.Where(c => c.children.Any(x => x is T)).FirstOrDefault();
        }

        public override ITerm VisitFilter([NotNull] ODataGrammarParser.FilterContext context)
        {
            var filterExpression = Visit(context.filterexpr);

            var termFilter = new MethodTerm(_operations.Where, new[] { new LambdaTerm(filterExpression) });
            return termFilter;
        }

        public override ITerm VisitSelect([NotNull] SelectContext context)
        {
            var selectArgs = context.children.OfType<SelectItemContext>().Select(c => Visit(c)).ToList();
            var select = new MethodTerm(_operations.Select, new[] { new LambdaTerm(new MethodTerm(OperationsMap.New, selectArgs.ToArray())) });
            return select;
        }

        public override ITerm VisitSelectItem([NotNull] SelectItemContext context)
        {
            return new SequenceTerm(
                ContextTerm.Root, 
                new PropertyTerm(context.GetText())
            );
        }


        public override ITerm VisitNotExpression([NotNull] NotExpressionContext context)
        {
            return new SequenceTerm(
                Visit(context.expression()),
                new MethodTerm(OperationsMap.Not)
            );
        }

        public override ITerm VisitStringExpression([NotNull] ODataGrammarParser.StringExpressionContext context)
        {
            return new ConstantTerm(context.GetText().Trim('\''));
        }

        public override ITerm VisitDecimalExpression([NotNull] ODataGrammarParser.DecimalExpressionContext context)
        {
            return new ConstantTerm(decimal.Parse(context.GetText()));
        }

        public override ITerm VisitIntExpression([NotNull] ODataGrammarParser.IntExpressionContext context)
        {
            return new ConstantTerm(int.Parse(context.GetText()));
        }

        public override ITerm VisitComparatorExpression([NotNull] ODataGrammarParser.ComparatorExpressionContext context)
        {
            var left = Visit(context.left);
            var right = Visit(context.right);

            var op = (MethodTerm)Visit(context.op);

            return new SequenceTerm(
                left,
                new MethodTerm(op.Operation, right)
            );
        }

        public override ITerm VisitIdentifierExpression([NotNull] ODataGrammarParser.IdentifierExpressionContext context)
        {
            if (string.IsNullOrEmpty(_currentContext) || (context.prefix?.Text?.Equals(_currentContext) ?? false))
                return new SequenceTerm
                (
                    ContextTerm.Root,
                    new PropertyTerm(context.val.Text)
                );
            else
                return new SequenceTerm
                (
                    ContextTerm.Root
                );
        }

        public override ITerm VisitParenExpression([NotNull] ODataGrammarParser.ParenExpressionContext context)
        {
            return Visit(context.children[1]);
        }

        public override ITerm VisitBoolExpression([NotNull] ODataGrammarParser.BoolExpressionContext context)
        {
            return new ConstantTerm(bool.Parse(context.GetText()));
        }

        public override ITerm VisitBinaryExpression([NotNull] ODataGrammarParser.BinaryExpressionContext context)
        {
            var left = Visit(context.left);
            var right = Visit(context.right);

            var op = (MethodTerm)VisitBinary(context.op);
            return new MethodTerm(op.Operation, left, right);
        }

        public override ITerm VisitTerminal(ITerminalNode node)
        {
            IOperation operation;
            switch (node.Symbol.Type)
            {
                case ODataGrammarParser.AND:
                    operation = OperationsMap.Every;
                    break;
                case ODataGrammarParser.OR:
                    operation = OperationsMap.OneOf;
                    break;

                case ODataGrammarParser.EQ:
                    operation = OperationsMap.Equal;
                    break;

                case ODataGrammarParser.NE:
                    operation = OperationsMap.NotEqual;
                    break;

                case ODataGrammarParser.LT:
                    operation = OperationsMap.LessThan;
                    break;
                case ODataGrammarParser.GT:
                    operation = OperationsMap.GreaterThan;
                    break;
                case ODataGrammarParser.LE:
                    operation = OperationsMap.LessThanOrEqual;
                    break;
                case ODataGrammarParser.GE:
                    operation = OperationsMap.GreaterThanOrEqual;
                    break;
                case ODataGrammarParser.Eof:
                    return base.VisitTerminal(node);
                default:
                    throw new Exception("Operation not supported");
            }
            return new MethodTerm(operation);
        }

        public override ITerm VisitFuncCallExpression([NotNull] FuncCallExpressionContext context)
        {
            var parameters = context.functionParams().children.Where((c, i) => i % 2 == 0).Select(c => Visit(c));
            if (!parameters.Any()) throw new ArgumentException("Need more arguments!");
            var funcRoot = parameters.First();
            var func = GetFuncTerm(context.func.Text);

            return new SequenceTerm(
                funcRoot
                , new MethodTerm(func.Operation, parameters.Skip(1).ToArray())
                );
        }

        public override ITerm VisitFunctionParams([NotNull] FunctionParamsContext context)
        {
            return base.VisitFunctionParams(context);
        }


        public override ITerm VisitCount([NotNull] CountContext context)
        {
            if (context.decexpr.GetText().Equals("true", StringComparison.OrdinalIgnoreCase))
                return new MethodTerm(_operations.Count);
            else return null;
        }

        public override ITerm VisitOrderby([NotNull] OrderbyContext context)
        {
            var args = context.children.OfType<OrderbyItemContext>().Select(c => Visit(c)).ToList();

            return new MethodTerm(_operations.Order, args.Select(a => new LambdaTerm(a)).ToArray());
        }

        public override ITerm VisitOrderbyItem([NotNull] OrderbyItemContext context)
        {
            var ordersq = new SequenceTerm(
                ContextTerm.Root,
                new PropertyTerm(context.children[0].GetText())
                );

            if (context.ChildCount > 1)
                ordersq = ordersq.Append(Visit(context.children[1]));

            return ordersq;
        }

        public override ITerm VisitOrder([NotNull] OrderContext context)
        {
            ITerm method;
            switch (context.GetText())
            {
                case "asc":
                    method = null;
                    break;
                case "desc":
                    method = new MethodTerm(OperationsMap.Reverse);
                    break;
                default:
                    throw new Exception($"Order direction {context.GetText()} is unknown");

            }
            return method;
        }

        public override ITerm VisitSkip([NotNull] SkipContext context)
        {
            if (int.TryParse(context.INT().GetText(), out var value) && value > 0)
                return new MethodTerm(_operations.Skip, new[] { new ConstantTerm(value) });
            else return null;
        }

        public override ITerm VisitTop([NotNull] TopContext context)
        {
            if (int.TryParse(context.INT().GetText(), out var value) && value > 0)
                return new MethodTerm(_operations.Top, new[] { new ConstantTerm(value) });
            else return null;
        }

        public override ITerm VisitDatetimeExpression([NotNull] DatetimeExpressionContext context)
        {
            return new ConstantTerm(context.GetText());
        }

        public override ITerm VisitGdExpression([NotNull] GdExpressionContext context)
        {
            return new ConstantTerm(context.GetText());
        }

        public override ITerm VisitLambda([NotNull] LambdaContext context)
        {
            var prevContext = _currentContext;

            var field = new SequenceTerm
                (
                    ContextTerm.Root,
                    new PropertyTerm(context.fld.Text)
                );

            _currentContext = context.lid.Text;
            var result = Visit(context.laExpr);
            _currentContext = prevContext;

            var lname = context.lambdaName().GetText().Equals("any") ? OperationsMap.Any : OperationsMap.All;

            var lTerm = new SequenceTerm
                (
                    field,
                    new MethodTerm(lname, new[] { new LambdaTerm(result) })
                );

            return lTerm;
        }

        public override ITerm VisitNullExpression([NotNull] NullExpressionContext context)
        {
            return new ConstantTerm(null);
        }

        private MethodTerm GetFuncTerm(string funcName)
        {
            IOperation operation;

            switch (funcName)
            {
                //case "IsEmpty":
                //    return typeof(string).GetMethod("IsNullOrWhiteSpace", new Type[] { typeof(string) });
                case "contains":
                    operation = OperationsMap.Has;
                    break;
                default:
                    throw new Exception($"Function {funcName} not found");
            }

            return new MethodTerm(operation);
        }

    }
}
