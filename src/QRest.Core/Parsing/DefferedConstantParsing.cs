namespace QRest.Core.Parsing
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
}

