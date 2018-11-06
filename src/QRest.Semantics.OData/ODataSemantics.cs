using Antlr4.Runtime;
using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Terms;
using QRest.OData;
using System.Linq;

namespace QRest.Semantics.OData
{
    public class ODataSemantics : IQuerySemanticsProvider
    {

        public LambdaTerm Parse(IRequestModel model)
        {
            var clauses = new[] { "$filter", "$select", "$orderby", "$count", "$top", "$skip" };

            var strings = clauses.Where(c => model.GetNamedQueryPart(c).Length > 0)
                .Select(c => $"{c}={model.GetNamedQueryPart(c).ToArray().First()}");

            ICharStream stream = CharStreams.fromstring(string.Join("&",strings));
            ITokenSource lexer = new ODataGrammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            var parser = new ODataGrammarParser(tokens);
            var context = parser.parse();

            var vis = new ODataVisitor();
            var exp = vis.Visit(context);

            var lambda = new LambdaTerm(BuiltIn.Roots.OriginalRoot, exp);

            return lambda;
        }
    }
}
