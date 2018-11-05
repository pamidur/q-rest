using QRest.Compiler.Standard.StringParsing;

namespace QRest.Compiler.Standard.Assembler
{
    public interface IAssemblerOptions
    {
        bool TerminateAfterSelect { get; set; }
        bool TerminateSequence { get; set; }
        IStringParsingBehavior StringParsing { get; set; }
    }
}
