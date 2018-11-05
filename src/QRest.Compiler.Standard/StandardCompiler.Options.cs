using QRest.Compiler.Standard.Assembler;
using QRest.Compiler.Standard.StringParsing;

namespace QRest.Compiler.Standard
{
    public partial class StandardCompiler : IAssemblerOptions
    {
        public bool TerminateAfterSelect { get; set; } = true;
        public bool TerminateSequence { get; set; } = true;
        public IStringParsingBehavior StringParsing { get; set; } = new ParseStringsToClrTypes();
    }
}
