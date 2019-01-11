using QRest.AspNetCore.Contracts;
using QRest.Core.Terms;
using System.Collections.Generic;

namespace QRest.AspNetCore.Native
{
    class NativeQueryStructure : IQueryStructure
    {
        public RootTerm Data { get; set; }
        public IReadOnlyList<RootTerm> GetAll() => new[] { Data };
    }
}
