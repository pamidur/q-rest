using System;
using System.IO;

namespace QRest.Core.Contracts
{
    public interface IRequestModel
    {
        string ModelName { get; }
        ReadOnlyMemory<string> GetNamedQueryPart(string name);
        Stream GetBody();
    }
}
