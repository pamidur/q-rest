using Antlr4.Runtime;
using QRest.Core.Exceptions;
using System.IO;

namespace QRest.AspNetCore.OData
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
