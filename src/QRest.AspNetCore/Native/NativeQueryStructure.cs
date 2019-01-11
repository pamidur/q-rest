using QRest.AspNetCore.Contracts;
using QRest.Core.Terms;
using System.Collections.Generic;

namespace QRest.AspNetCore.Native
{
    class NativeQueryStructure : IQueryStructure
    {
        public LambdaTerm Data { get; set; }
        public IReadOnlyList<LambdaTerm> GetAll() => new[] { Data };
    }
}
