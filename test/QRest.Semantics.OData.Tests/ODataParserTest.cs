using Antlr4.Runtime;
using QRest.Core.Contracts;
using Xunit;

namespace QRest.OData.Tests
{
    public class ODataParserTest
    {
        [Theory]
        [InlineData("-where(|ce>-it.param1-eq('[L72]'))-new(-ctx@value)", @"$filter =   param1 eq '[L72]'")]
        [InlineData("-where(|ce>'L72'-eq(-it.param1))-new(-ctx@value)", @"$filter =  'L72' eq param1 ")]
        [InlineData("-where(|ce>-every(-it.param1-eq('L72'),-oneof(-it.param2-eq('qwerty'),-it.param3-eq('asdf'))))-new(-ctx@value)",
            @"$filter = param1 eq 'L72' AND (param2 eq 'qwerty' OR param3 eq 'asdf') ")]
        public void ShouldParseFilterQueryOption(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.SharedView);
        }

        [Fact]
        public void ShouldParseFuncAndNOT()
        {
            var input = @"$filter = not contains(param,'b')";
            ITerm exp = Parse(input);
            Assert.Equal("-where(|ce>-it.param-contains('b')-not)-new(-ctx@value)", exp.SharedView);

        }

        [Theory]
        [InlineData("-where(|ce>-it.a-eq(-it.b))-new(-count@@odata.count,-ctx@value)", @"$filter = a eq b&$count=true")]
        [InlineData("-where(|ce>-it.a-eq(-it.b))-new(-ctx@value)", @"$filter = a eq b&$count=false")]
        public void ShouldParseCount(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.SharedView);
        }


        [Fact]
        public void ShouldParseEmptyString()
        {
            ITerm exp = Parse(string.Empty);
            Assert.Equal("-new(-ctx@value)", exp.SharedView);
        }

        [Theory]
        [InlineData("-where(|ce>-it.a-eq(-it.b))-new(-select(|ce>-new(-it.f1,-it.f2))@value)", @"$filter = a eq b&$count=false&$select=f1,f2")]
        [InlineData("-where(|ce>-it.a-eq(-it.b))-new(-count@@odata.count,-select(|ce>-new(-it.f1,-it.f2))@value)", @"$filter = a eq b&$count=true&$select=f1,f2")]
        public void ShouldParseSelect(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.SharedView);
        }

        [Theory]
        [InlineData("-new(-skip('1')-take('1')-select(|ce>-new(-it.f1,-it.f2))@value)", @"$count=false&$select=f1,f2&$skip=1&$top=1")]
        public void ShouldParseTopSkip(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.SharedView);
        }


        [Theory]
        [InlineData("-new(-order(|ce>-it.f1-ctx,|ce>-it.f2-ctx)@value)", @"$orderby=f1,f2")]
        [InlineData("-new(-order(|ce>-it.f1-ctx,|ce>-it.f2-!rev)@value)", @"$orderby=f1 asc,f2 desc" )]
        [InlineData("-new(-count@@odata.count,-order(|ce>-it.f1-ctx,|ce>-it.f2-!rev)@value)", @"$orderby=f1 asc,f2 desc&$count=true")]
        public void ShouldParseOrderBy(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.SharedView);
        }


        private static ITerm Parse(string input)
        {
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new ODataGrammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            var parser = new ODataGrammarParser(tokens);
            var context = parser.parse();

            var vis = new ODataVisitor();
            var exp = vis.Visit(context);
            return exp;
        }
    }
}
