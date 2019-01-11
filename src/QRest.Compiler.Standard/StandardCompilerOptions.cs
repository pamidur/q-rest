using QRest.Compiler.Standard.StringParsing;

namespace QRest.Compiler.Standard
{
    public class StandardCompilerOptions
    {
        public bool AllowUncompletedQueries { get; set; } = false;
        public IStringParsingBehavior StringParsing { get; set; } = new ParseStringsToClrTypes();
        public bool UseCompilerCache { get; set; } = true;
    }
}
