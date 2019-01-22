using Antlr4.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QRest.AspNetCore.Contracts;
using QRest.Core.Terms;
using QRest.OData;
using QRest.Semantics.OData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.AspNetCore.OData
{
    public class ODataSemantics : ISemantics
    {
        private static readonly Type _queryableIface = typeof(IQueryable<>);
        private static readonly string _queryableIfaceName = $"{_queryableIface.Namespace}.{_queryableIface.Name}";

        private ODataOptions _options;

        public ODataSemantics(IOptions<ODataOptions> options)
        {
            _options = options.Value;
        }

        public IQueryStructure ReadQueryStructure(IReadOnlyList<string> values, HttpRequest request)
        {
            var clauses = new[] { "$filter", "$select", "$orderby", "$count", "$top", "$skip" };

            var strings = new List<string>();

            foreach (var c in clauses)
                if (request.Query.TryGetValue(c, out var strs))
                    strings.Add($"{c}={strs.First()}");

            ICharStream stream = CharStreams.fromstring(string.Join("&", strings));
            ITokenSource lexer = new ODataGrammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            var parser = new ODataGrammarParser(tokens);
            var context = parser.parse();

            var vis = new ODataVisitor();
            var exp = vis.Visit(context);

            var container = (ODataTermContainer)exp;

            var result = new ODataQueryStructure()
            {
                Data = new RootTerm(container.Data)
            };

            if (container.Count != null)
                result.Count = new RootTerm(container.Count);

            return result;
        }

        public ActionResult WriteQueryResponse(IQueryStructure query, IReadOnlyDictionary<RootTerm, object> results)
        {
            var odataquery = (ODataQueryStructure)query;
            return new ODataQueryResult(odataquery, results, _options?.MetadataPath);
        }
    }
}
