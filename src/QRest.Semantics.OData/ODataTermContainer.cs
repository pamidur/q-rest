using QRest.Core.Contracts;
using QRest.Core.Terms;

namespace QRest.Semantics.OData
{
    public class ODataTermContainer : SequenceTerm
    {
        public ITerm Data { get; set; }
        public ITerm Count { get; set; }

        public override string SharedView
        {
            get
            {
                var result = $"value={Data.SharedView}";

                if (Count != null)
                    result += $";count={Count.SharedView}";

                return result;
            }
        }
    }
}
