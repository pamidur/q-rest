using Antlr4.Runtime;
using System;
using Xunit;

namespace QRest.OData.Tests
{
    public class ODataParserTest
    {
        [Fact]
        public void ShouldParseEqFilterQueryOption()
        {
            var input = @"$filter =  'L72' eq param1 ";

            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new ODataGrammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            var parser = new ODataGrammarParser(tokens);
            var context = parser.filter();

            var vis = new ODataVisitor();
            var exp = vis.Visit(context);


        }

        [Fact]
        public void ShouldParseANDFilterQueryOption()
        {
            var input = @"$filter = param1 eq 'L72' AND param2 eq 'qwerty'";

            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new ODataGrammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            var parser = new ODataGrammarParser(tokens);
            var context = parser.filter();

            var vis = new ODataVisitor();
            var exp = vis.Visit(context);


        }
    }
}
