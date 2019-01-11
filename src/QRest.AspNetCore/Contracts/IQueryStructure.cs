using QRest.Core.Terms;
using System.Collections.Generic;

namespace QRest.AspNetCore.Contracts
{
    public interface IQueryStructure
    {
        LambdaTerm Data { get; }
        IReadOnlyList<LambdaTerm> GetAll();
    }
}
