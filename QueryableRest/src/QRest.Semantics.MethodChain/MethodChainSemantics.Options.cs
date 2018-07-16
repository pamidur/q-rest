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
        /// Configures Select() to be executed with with following ToArray(). Useful for Qury Providers that don't support dynamic return e.g. EF6, MongoDriver.
        /// True by default
        /// </summary>
        public bool UseStaticQueryTerminator { get; set; } = true;

        /// <summary>
        /// Allows using TypeName.Parse(string) static methods in the place of comparison operations.
        /// Default value is <see cref="DefferedConstantParsing.Strings" />
        /// </summary>
        public DefferedConstantParsing UseDefferedConstantParsing { get; set; } = DefferedConstantParsing.Strings;

        public Dictionary<string, IOperation> CustomOperations { get; set; } = new Dictionary<string, IOperation>();


        /// <summary>
        /// Custom parsers for types without TypeName.Parse(string) method.
        /// Requires minimum deffered setting <see cref="UseDefferedConstantParsing" />=<see cref="DefferedConstantParsing.Strings" />
        /// </summary>
        public Dictionary<Type, Func<string, object>> CustomConstantParsers { get; set; } = new Dictionary<Type, Func<string, object>>();
    }
}
