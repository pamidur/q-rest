using QRest.Core.Contracts;
using QRest.Core.Operations.Basic;

namespace QRest.Core.Terms
{
    public class PropertyTerm : TermBase
    {
        private readonly string _name;
        public PropertyTerm(string name) => _name = name;

        public override IOperation Operation => new GetPropertyOperation(_name);        

        public override string SharedView => $".{_name}";
        public override ITerm Clone() => new PropertyTerm (_name);
    }
}
