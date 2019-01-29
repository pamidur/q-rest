using System;
using System.Reflection;

namespace QRest.Compiler.Standard.Assembler
{
    class TypeConverters
    {
        public static readonly MethodInfo DateTimeToUtc = typeof(TypeConverters).GetMethod(nameof(ToUtc));

        public static DateTime ToUtc (DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime.ToUniversalTime();

            if (dateTime.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            return dateTime;
        }
    }
}
