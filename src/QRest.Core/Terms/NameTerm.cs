using QRest.Core.Contracts;
using QRest.Core.Operations.Basic;

namespace QRest.Core.Terms
{
    public class NameTerm : TermBase
    {
        private readonly string _name;
        public NameTerm(string name) => _name = name;

        public override IOperation Operation => new SetNameOperation(_name);

        public override string SharedView => $"@{_name}";
        public override ITerm Clone() => new NameTerm(_name);
    }
}
