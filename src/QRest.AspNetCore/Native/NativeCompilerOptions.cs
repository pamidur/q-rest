using System;
using System.Globalization;

namespace QRest.AspNetCore.Native
{
    public class NativeCompilerOptions
    {
        public DateTimeKind AssumeDateTimeKind { get; set; } = DateTimeKind.Utc;
        public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;
        public bool UseCompilerCache { get; set; } = true;
    }
}
