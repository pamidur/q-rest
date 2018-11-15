using QRest.Compiler.Standard;
using QRest.Core.Contracts;
using QRest.Semantics.QRest;

namespace QRest.AspNetCore
{
    public class QRestOptions
    {
        public ISemantics Semantics { get; set; } = new QRestSemantics();
        public ICompiler Compiler { get; set; } = new StandardCompiler();
    }
}
