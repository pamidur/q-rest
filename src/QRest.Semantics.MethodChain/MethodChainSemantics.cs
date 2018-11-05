using QRest.Core;
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
        private static readonly LambdaTerm _default = new LambdaTerm(BuiltIn.Roots.OriginalRoot, new MethodTerm(new ItOperation()).AsSequence());

        private Lazy<Parser<LambdaTerm>> Parser { get; }

        public MethodChainSemantics()
        {
            Parser = new Lazy<Parser<LambdaTerm>>(() => PrepareParser());
        }

        private Parser<LambdaTerm> PrepareParser()
        {
            return new MethodChainParserBuilder(UseDefferedConstantParsing, _callMap, _queryMap).Build();
        }

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
