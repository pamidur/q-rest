using QRest.Core.Contracts;
using QRest.Core.Operations;
using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using QRest.Core.Operations.Query.OrderDirectionOperations;
using QRest.Core.Operations.Selectors;
using QRest.Core.RootProviders;
using QRest.Core.Terms;
using Sprache;
using System;
using System.Collections.Generic;

namespace QRest.Semantics.MethodChain
{
    public partial class MethodChainSemantics : IQuerySemanticsProvider
    {
        private static readonly LambdaTerm _default = new LambdaTerm(BuiltInRootProviders.Root) { new MethodTerm(new ItOperation()) };

        private readonly Dictionary<string, IOperation> _callMap = new Dictionary<string, IOperation>
        {
            {"ne", new NotEqualOperation() },
            {"eq", new EqualOperation()},
            {"gt", new GreaterThanOperation()},
            {"gte", new GreaterThanOrEqualOperation()},
            {"lt", new LessThanOperation()},
            {"lte", new LessThanOrEqualOperation()},
            {"not", new NotOperation()},
            {"oneof", new OneOfOperation() },
            {"every", new EveryOperation() },
            { "has", new ContainsOperation() },
            { "get", new NewOperation() },

            {"asc", new AscendingOperation()},
            {"desc", new DescendingOperation()},

            {"it", new ItOperation()},
            {"ctx", new ContextOperation()},
        };

        private readonly Dictionary<string, IOperation> _queryMap = new Dictionary<string, IOperation>
        {
            {"where", new WhereOperation()},
            {"get", new SelectOperation()},
            {"first", new FirstOperation()},
            {"count", new CountOperation()},
            {"sum", new SumOperation()},
            {"skip", new SkipOperation()},
            {"take", new TakeOperation()},
            {"order", new OrderOperation()},
        };

        private Lazy<Parser<LambdaTerm>> Parser { get; }

        public MethodChainSemantics()
        {
            Parser = new Lazy<Parser<LambdaTerm>>(() => PrepareParser());
        }

        private Parser<LambdaTerm> PrepareParser()
        {
            //var a = new MethodChainParserBuilder(UseDefferedConstantParsing, _operationMap);
            //a.Build();
            //var m = a.CallChain.Parse("-");

            return new MethodChainParserBuilder(UseDefferedConstantParsing, _callMap, _queryMap).Build();
        }

        //protected void AddOperation(string name, IOperation operation)
        //{
        //    if (operation is OperationBase && UseDefferedConstantParsing >= DefferedConstantParsing.Strings)
        //    {
        //        var compareOp = (OperationBase)operation;

        //        compareOp.TryParseFromStrings = true;

        //        foreach (var parser in CustomConstantParsers)
        //            compareOp.Parsers.Add(parser.Key, parser.Value);
        //    }

        //    _callMap.Add(name, operation);
        //}

        public LambdaTerm Parse(IRequestModel model)
        {
            var queries = model.GetNamedQueryPart(model.ModelName).Span;

            if (queries.Length == 0)
                return _default;

            var result = Parser.Value.TryParse(queries[0]);

            return result.Value;
        }
    }
}
