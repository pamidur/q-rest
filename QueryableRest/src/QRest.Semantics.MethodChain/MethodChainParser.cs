using QRest.Core.Contracts;
using System.Collections.Generic;
using QRest.Core.Terms;
using Sprache;
using System.Linq;
using QRest.Core.Operations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using QRest.Core.Operations.Aggregations;
using QRest.Core.Operations.Query.OrderDirectionOperations;
using QRest.Core;
using System;

namespace QRest.Semantics.MethodChain
{
    public partial class MethodChainParser : IQuerySemanticsProvider
    {
        private readonly Dictionary<string, IOperation> _operationMap = new Dictionary<string, IOperation>();

        internal readonly Parser<ITerm> _parser;

        public MethodChainParser(Action<MethodChainParserOptions> optionsEditor = null)
        {
            var options = new MethodChainParserOptions();

            if (optionsEditor != null)
                optionsEditor(options);

            PrepareParser(options);

            _parser = CallChain(this);
        }

        private void PrepareParser(MethodChainParserOptions options)
        {
            AddDefaultOperations(options);

            foreach (var customOp in options.CustomOperations)
                AddOperation(customOp.Key, customOp.Value, options);
        }

        private void AddDefaultOperations(MethodChainParserOptions options)
        {
            AddOperation("ne", new NotEqualOperation(), options);
            AddOperation("eq", new EqualOperation(), options);
            AddOperation("not", new NotOperation(), options);
            AddOperation("where", new WhereOperation(), options);
            AddOperation("get", new SelectOperation { UseStaticTerminatingQuery = options.UseStaticQueryTerminator }, options);
            AddOperation("oneof", new OneOfOperation(), options);
            AddOperation("every", new EveryOperation(), options);
            AddOperation("first", new FirstOperation(), options);
            AddOperation("count", new CountOperation(), options);
            AddOperation("sum", new SumOperation(), options);
            AddOperation("has", new ContainsOperation(), options);
            AddOperation("skip", new SkipOperation(), options);
            AddOperation("take", new TakeOperation(), options);
            AddOperation("order", new OrderOperation(), options);
            AddOperation("asc", new AscendingOperation(), options);
            AddOperation("desc", new DescendingOperation(), options);


            AddOperation("with", new WithOperation(), options);
            AddOperation("it", new ItOperation(), options);
        }

        protected void AddOperation(string name, IOperation operation, MethodChainParserOptions options)
        {
            if (operation is CompareOperationBase && options.UseDefferedConstantParsing >= DefferedConstantParsing.Strings)
            {
                var compareOp = (CompareOperationBase)operation;

                compareOp.TryParseFromStrings = true;

                foreach (var parser in options.CustomConstantParsers)
                    compareOp.Parsers.Add(parser.Key, parser.Value);
            }

            _operationMap.Add(name, operation);
        }

        public ITerm Parse(IReadOnlyDictionary<string, string[]> queryParts)
        {
            var query = queryParts.FirstOrDefault().Value.FirstOrDefault();

            if (string.IsNullOrEmpty(query))
                return null;

            var result = _parser.TryParse(queryParts.First().Value[0]);

            return result.Value;
        }

        public string[] QuerySelector(string modelname)
        {
            return new[] { modelname };
        }

        protected IOperation SelectOperation(string opName)
        {
            if (!_operationMap.TryGetValue(opName, out var op))
                throw new ExpressionParsingException();

            return op;
        }
    }
}
