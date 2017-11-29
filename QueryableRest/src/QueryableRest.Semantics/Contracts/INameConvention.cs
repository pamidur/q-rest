using System;
using System.Collections.Generic;
using System.Text;

namespace QueryableRest.Semantics.Contracts
{
    public interface INameConvention
    {
        string DtoToModel(string name);
        string ModelToDto(string name);
    }
}
