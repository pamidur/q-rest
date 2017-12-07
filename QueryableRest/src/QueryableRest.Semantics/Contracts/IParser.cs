using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QRest.Core.Contracts
{
    public interface IQuerySemanticsProvider
    {
        bool QuerySelector(string queryparam, string modelname);

        ITerm Parse(IReadOnlyDictionary<string, string[]> queryParts);
    }
}
