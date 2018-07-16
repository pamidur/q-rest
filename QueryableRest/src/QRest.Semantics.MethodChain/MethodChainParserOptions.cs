using QRest.Core.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace QRest.Semantics.MethodChain
{
    public enum DefferedConstantParsing: byte
    {
        /// <summary>
        /// Don't use deffered parsing.
        /// </summary>
        None = 0,
        /// <summary>
        /// Use only for string values. Allows '{Guid}' and '{DateTime}' to be parsed.
        /// </summary>
        Strings = 1,
        /// <summary>
        /// Use for all values. Useful for cases when need additional precision for decimals and doubles
        /// </summary>
        All = 2
    }

    public class MethodChainParserOptions
    {
        /// <summary>
        /// Configures Select() to be executed with with following ToArray(). Useful for Qury Providers that don't support dynamic return e.g. EF6, MongoDriver.
        /// True by default
        /// </summary>
        public bool UseStaticQueryTerminator { get; set; } = true;

        /// <summary>
        /// Allows using TypeName.Parse(string) static methods in the place of comparison operations.
        /// </summary>
        public DefferedConstantParsing UseDefferedConstantParsing { get; set; } = DefferedConstantParsing.Strings;

        public Dictionary<string, IOperation> CustomOperations { get; set; } = new Dictionary<string, IOperation>();

        public Dictionary<Type, Func<string, object>> CustomConstantParsers { get; set; } = new Dictionary<Type, Func<string, object>>();
    }
}
