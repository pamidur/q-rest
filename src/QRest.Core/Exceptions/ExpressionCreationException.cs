using System;
using System.Collections.Generic;
using System.Text;

namespace QRest.Core
{
    public class ExpressionCreationException : Exception
    {
        public ExpressionCreationException()
        {

        }

        public ExpressionCreationException(string message) : base(message)
        {
        }
    }
}
