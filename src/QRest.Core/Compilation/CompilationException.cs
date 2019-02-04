using System;
namespace QRest.Core.Compilation
{
    public class CompilationException : Exception
    {
        public CompilationException(string message) : base(message)
        {
        }
    }
}
