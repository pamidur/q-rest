using QRest.Core.Contracts;
using QRest.Core.Operations.Basic;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public class ConstantTerm : TermBase
    {
        public object Value { get; set; }

        public override IOperation Operation => new CreateConstantOperation(Value, Value.GetType());

        public override string SharedView => $"'{Value.ToString()}'";
        public override string KeyView => $"[{Value.GetType()}]";
        public override ITerm Clone() => new ConstantTerm { Value = Value };
    }
}
