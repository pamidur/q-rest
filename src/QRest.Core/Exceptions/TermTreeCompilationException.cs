using System;
using System.Collections.Generic;
using System.Text;

namespace QRest.Core.Exceptions
{
    public class TermTreeCompilationException : Exception
    {
        public TermTreeCompilationException(string message) : base(message)
        {
        }
    }
}
