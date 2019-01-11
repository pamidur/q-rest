using System.Collections.Generic;
using QRest.AspNetCore.Contracts;
using QRest.Core.Terms;

namespace QRest.AspNetCore.OData
{
    public class ODataQueryStructure : IQueryStructure
    {
        public LambdaTerm Data { get; set; }
        public LambdaTerm Count { get; set; }

        public IReadOnlyList<LambdaTerm> GetAll() => new[] { Data, Count };
    }
}
