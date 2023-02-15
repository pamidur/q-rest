using QRest.Core.Terms;
using System;

namespace QRest.Semantics.OData.Parsing
{
    public class ODataTermContainer : ITerm
    {
        public ITerm Data { get; set; }
        public ITerm Count { get; set; }

        public string ViewQuery => FormatView(t => t.ViewQuery);

        public string ViewDebug => FormatView(t=>t.ViewDebug);

        public string ViewKey => FormatView(t => t.ViewKey);

        public string FormatView(Func<ITerm, string> selector)
        {
            var result = $"value={selector(Data)}";

            if (Count != null)
                result += $";count={selector(Count)}";

            return result;
        }

        public ITerm Clone()
        {
            throw new System.NotSupportedException();
        }
    }
}
