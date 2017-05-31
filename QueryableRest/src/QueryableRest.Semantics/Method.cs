using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryableRest.Semantics
{
    public abstract class Method<TTarget, TOperation>
        where TOperation : Operation<TTarget>
    {
        public abstract ICollection<TOperation> Operations { get; }

        public virtual string Serialize()
        {
            return string.Join(Constants.OperationBlockSeparator, Operations.Select(o => o.Serialize()));
        }

        protected static string[] GetBlocks(string data)
        {
            return data.Split(new string[] { Constants.OperationBlockSeparator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public override string ToString()
        {
            return Serialize();
        }
    }
}
