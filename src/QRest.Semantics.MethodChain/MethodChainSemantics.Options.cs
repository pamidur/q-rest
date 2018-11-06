using QRest.Core.Contracts;
using QRest.Core.Operations;
using System;
using System.Collections.Generic;

namespace QRest.Semantics.MethodChain
{
    public enum DefferedConstantParsing : byte
    {
        /// <summary>
        /// Don't use deffered parsing.
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// Use only for string values. Allows '{Guid}' and '{DateTime}' to be parsed.
        /// </summary>
        Strings = 1,
        /// <summary>
        /// Use for strings and number values. Useful for cases when need long int suport and additional precision for decimals and doubles
        /// </summary>
        StringsAndNumbers = 3,
        /// <summary>
        /// Use for all values
        /// </summary>
        All = 4,
    }

    public partial class MethodChainSemantics
    {
        /// <summary>
        /// Allows using TypeName.Parse(string) static methods in the place of comparison operations.
        /// Default value is <see cref="DefferedConstantParsing.StringsAndNumbers" />
        /// </summary>
        public DefferedConstantParsing UseDefferedConstantParsing { get; set; } = DefferedConstantParsing.StringsAndNumbers;

        //public Dictionary<string, IOperation> CustomOperations { get; set; } = new Dictionary<string, IOperation>();
    }
}
