using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QRest.Core.Contracts
{
    public interface IQuerySemanticsProvider
    {
        string[] QuerySelector(string modelname);

        ITerm Parse(IReadOnlyDictionary<string, string[]> queryParts);
    }
}
