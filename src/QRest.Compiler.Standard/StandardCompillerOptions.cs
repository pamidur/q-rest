namespace QRest.Compiler.Standard
{
    public class StandardCompillerOptions
    {
        public bool TerminateAfterSelect { get; set; } = true;
        public bool TerminateSequence { get; set; } = true;
        public IStringParsingBehavior StringParsing { get; set; } = new ParseStringsToClrTypes();
    }
}
