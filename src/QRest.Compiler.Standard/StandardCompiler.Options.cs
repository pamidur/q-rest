using QRest.Compiler.Standard.Assembler;
using QRest.Compiler.Standard.StringParsing;

namespace QRest.Compiler.Standard
{
    public partial class StandardCompiler : IAssemblerOptions
    {
        public bool AllowUncompletedQueries { get; set; } = false;
        public IStringParsingBehavior StringParsing { get; set; } = new ParseStringsToClrTypes();
    }
}
