using System;

namespace QRest.Compiler.Standard.StringParsing
{
    public static class Parsers
    {
        public static DateTime ParseDateTime(string source, IFormatProvider format)
        {
            return DateTime.Parse(source, format);
            //return DateTimeOffset.Parse(source).DateTime;
        }
    }
}
