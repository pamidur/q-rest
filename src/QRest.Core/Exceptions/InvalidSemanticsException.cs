namespace QRest.Core.Exceptions
{
    public class InvalidSemanticsException : TermTreeParsingException
    {
        public InvalidSemanticsException(string message) : base(message)
        {
        }

        public string[] Expectations { get; set; }
        public int Position { get; set; }
    }
}
