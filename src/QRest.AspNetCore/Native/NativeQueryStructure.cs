using QRest.AspNetCore.Contracts;
using QRest.Core.Contracts;
using System.Collections.Generic;

namespace QRest.AspNetCore.Native
{
    class NativeQueryStructure : IQueryStructure
    {
        public ITerm Data { get; set; }
        public IReadOnlyList<ITerm> GetAll() => new[] { Data };
    }
}
