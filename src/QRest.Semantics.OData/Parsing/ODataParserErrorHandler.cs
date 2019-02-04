using Antlr4.Runtime;
using QRest.AspNetCore;
using System.IO;

namespace QRest.Semantics.OData.Parsing
{
    public class ODataParserErrorListener : BaseErrorListener
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new InvalidSemanticsException(msg) {
                Position = charPositionInLine
            };
        }
    }
}
