using QRest.AspNetCore;
using QRest.AspNetCore.Contracts;
using QRest.AspNetCore.Native;
using QRest.Compiler.Standard;
using QRest.Core;
using QRest.Core.Contracts;
using System.Collections.Generic;

namespace QRest.AspNetCore.Native
{
    public class NativeSemanticsOptions
    {
        /// <summary>
        /// Allows using TypeName.Parse(string) static methods in the place of comparison operations.
        /// Default value is <see cref="DefferedConstantParsing.StringsAndNumbers" />
        /// </summary>
        public DefferedConstantParsing UseDefferedConstantParsing { get; set; } = DefferedConstantParsing.StringsAndNumbers;

        public Dictionary<string, IOperation> CustomOperations { get; set; } = new Dictionary<string, IOperation>();
    }
}
