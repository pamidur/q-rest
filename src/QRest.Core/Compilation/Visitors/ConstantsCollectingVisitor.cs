using QRest.Core.Terms;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace QRest.Core.Compilation.Visitors
{    
    public class ConstantsCollectingVisitor : TermVisitor<ImmutableArray<ConstantExpression>>
    {
        public IReadOnlyList<ConstantExpression> Collect(ITerm lambda)
        {
            var list = Visit(lambda, ImmutableArray<ConstantExpression>.Empty);
            return list;
        }

        protected override ImmutableArray<ConstantExpression> VisitConstant(ConstantTerm c, in ImmutableArray<ConstantExpression> state)
        {
            var constant = Expression.Constant(c.Value);
            return state.Add(constant);
        }
    }
}
