using QRest.Core.Terms;

namespace QRest.Semantics.OData.Parsing
{
    public class ODataTermContainer : ITerm
    {
        public ITerm Data { get; set; }
        public ITerm Count { get; set; }

        public string SharedView
        {
            get
            {
                var result = $"value={Data.SharedView}";

                if (Count != null)
                    result += $";count={Count.SharedView}";

                return result;
            }
        }

        public string DebugView => SharedView;

        public string KeyView => SharedView;

        public ITerm Clone()
        {
            throw new System.NotSupportedException();
        }
    }
}
