using System;
using System.Collections.Generic;
using System.Text;

namespace QRest.Core.Exceptions
{
    public class UnknownOperationException : TermTreeParsingException
    {
        private string opName;

        public UnknownOperationException(string opName)
        {
            this.opName = opName;
        }
    }
}
