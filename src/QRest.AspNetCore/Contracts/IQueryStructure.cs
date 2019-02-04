using QRest.Core.Terms;
using System.Collections.Generic;

namespace QRest.AspNetCore.Contracts
{
    public interface IQueryStructure
    {
        ITerm Data { get; }
        IReadOnlyList<ITerm> GetAll();
    }
}
