using QRest.Compiler.Standard.StringParsing;

namespace QRest.Compiler.Standard.Assembler
{
    public interface IAssemblerOptions
    {
        bool AllowUncompletedQueries { get; set; }
        IStringParsingBehavior StringParsing { get; set; }
    }
}
