namespace QRest.Core.Exceptions
{
    public class InvalidSemanticsException : TermTreeParsingException
    {
        public InvalidSemanticsException(string message) : base(message)
        {
        }

        public string[] Expectations { get; set; } = new string[] { };
        public int Position { get; set; }
    }
}
