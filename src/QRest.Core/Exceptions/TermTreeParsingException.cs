using System;

namespace QRest.Core
{
    public abstract class TermTreeParsingException : Exception
    {
        protected TermTreeParsingException(string message):base(message)
        {
        }
    }
}
