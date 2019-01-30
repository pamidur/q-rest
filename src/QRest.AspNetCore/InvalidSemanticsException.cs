using System;

namespace QRest.AspNetCore
{
    public class InvalidSemanticsException : Exception
    {
        public InvalidSemanticsException(string message) : base(message)
        {
        }

        public string[] Expectations { get; set; }
        public int Position { get; set; }
    }
}
