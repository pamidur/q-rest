using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace QRest.AspNetCore.OData
{
    public class ODataOptions
    {
        public PathString MetadataPath { get; set; }
        public IReadOnlyList<PathString> ApiPaths { get; set; }
    }
}
