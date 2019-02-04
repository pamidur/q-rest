using QRest.Core.Operations;
using QRest.Core.Parsing;
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
