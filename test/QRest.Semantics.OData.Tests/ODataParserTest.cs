using Antlr4.Runtime;
using QRest.Core.Contracts;
using Xunit;

namespace QRest.OData.Tests
{
    public class ODataParserTest
    {
        [Theory]
        [InlineData("-where(:-$.param1-eq('[L72]'))-map(-$$@value)", @"$filter =   param1 eq '[L72]'")]
        [InlineData("-where(:'L72'-eq(-$.param1))-map(-$$@value)", @"$filter =  'L72' eq param1 ")]
        [InlineData("-where(:-every(-$.param1-eq('L72'),-oneof(-$.param2-eq('qwerty'),-$.param3-eq('asdf'))))-map(-$$@value)",
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
            Assert.Equal("-where(:-$.param-has('b')-not)-map(-$$@value)", exp.SharedView);

        }

        [Theory]
        [InlineData("-where(:-$.a-eq(-$.b))-map(-count@@odata.count,-$$@value)", @"$filter = a eq b&$count=true")]
        [InlineData("-where(:-$.a-eq(-$.b))-map(-$$@value)", @"$filter = a eq b&$count=false")]
        public void ShouldParseCount(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.SharedView);
        }


        [Fact]
        public void ShouldParseEmptyString()
        {
            ITerm exp = Parse(string.Empty);
            Assert.Equal("-map(-$$@value)", exp.SharedView);
        }

        [Theory]
        [InlineData("-where(:-$.a-eq('12/22/2019 21:02:00 +00:00'))-map(-$$@value)", @"$filter = a eq 2019-12-22T21:02:00.3434Z")]
        [InlineData("-where(:-$.a-eq('12/22/2019 21:02:00 +02:00'))-map(-$$@value)", @"$filter = a eq 2019-12-22T21:02:00.34346767+02:00")]
        [InlineData("-where(:-$.a-eq('12/22/2019 21:02:00 -03:00'))-map(-$$@value)", @"$filter = a eq 2019-12-22T21:02:00-03:00")]
        public void ShouldParseDateTimeOffset(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.SharedView);
        }

        [Theory]
        [InlineData("-where(:-$.a-eq(-$.b))-map(-each(:-map(-$.f1,-$.f2))@value)", @"$filter = a eq b&$count=false&$select=f1,f2")]
        [InlineData("-where(:-$.a-eq(-$.b))-map(-count@@odata.count,-each(:-map(-$.f1,-$.f2))@value)", @"$filter = a eq b&$count=true&$select=f1,f2")]
        public void ShouldParseeach(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.SharedView);
        }

        [Theory]
        [InlineData("-map(-skip('1')-take('1')-each(:-map(-$.f1,-$.f2))@value)", @"$count=false&$select=f1,f2&$skip=1&$top=1")]
        public void ShouldParseTopSkip(string expected, string input)
        {
            ITerm exp = Parse(input);
            Assert.Equal(expected, exp.SharedView);
        }


        [Theory]
        [InlineData("-map(-order(:-$.f1-$$,:-$.f2-$$)@value)", @"$orderby=f1,f2")]
        [InlineData("-map(-order(:-$.f1-$$,:-$.f2-desc)@value)", @"$orderby=f1 asc,f2 desc" )]
        [InlineData("-map(-count@@odata.count,-order(:-$.f1-$$,:-$.f2-desc)@value)", @"$orderby=f1 asc,f2 desc&$count=true")]
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
