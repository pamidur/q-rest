﻿using Antlr4.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QRest.AspNetCore.Contracts;
using QRest.Core.Terms;
using QRest.Semantics.OData.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QRest.Semantics.OData.Semantics
{
    public class ODataSemantics : ISemantics
    {
        public ODataSemantics(IOptions<ODataOptions> options)
        {
            _options = options?.Value ?? ODataOptions.Default;
        }

        private static readonly Type _queryableIface = typeof(IQueryable<>);
        private static readonly string _queryableIfaceName = $"{_queryableIface.Namespace}.{_queryableIface.Name}";
        private readonly ODataOptions _options;

        public IQueryStructure ReadQueryStructure(IReadOnlyList<string> values, HttpRequest request)
        {
            var clauses = new[] { "$filter", "$select", "$orderby", "$count", "$top", "$skip" };

            var strings = new List<string>();

            foreach (var c in clauses)
                if (request.Query.TryGetValue(c, out var strs))
                    strings.Add($"{c}={strs.First()}");

            ICharStream stream = CharStreams.fromString(string.Join("&", strings));
            ITokenSource lexer = new ODataGrammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            var parser = new ODataGrammarParser(tokens);
            parser.AddErrorListener(new ODataParserErrorListener());
            var context = parser.parse();

            var vis = new ODataVisitor(_options.Operations);
            var exp = vis.Visit(context);

            var container = (ODataTermContainer)exp;

            var result = new ODataQueryStructure()
            {
                Data = container.Data
            };

            if (container.Count != null)
                result.Count = container.Count;

            return result;
        }

        public ActionResult WriteQueryResponse(IQueryStructure query, IReadOnlyDictionary<ITerm, object> results, Type source)
        {
            var odataquery = (ODataQueryStructure)query;
            return new ODataQueryResult(odataquery, results);
        }
    }
}
