using QRest.Core.Terms;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Compilation.Visitors
{
    public readonly struct CollectingVisitorState
    {
        public readonly List<ConstantExpression> Constants;
        public CollectingVisitorState(List<ConstantExpression> initial)
        {
            Constants = initial;
        }
    }
    public class ConstantsCollectingVisitor : TermVisitor<CollectingVisitorState>
    {
        public IReadOnlyList<ConstantExpression> Collect(ITerm lambda)
        {
            var list = Visit(lambda, new CollectingVisitorState(new List<ConstantExpression>()));
            return list.Constants.ToArray();
        }

        protected override CollectingVisitorState VisitConstant(ConstantTerm c, in CollectingVisitorState state)
        {
            var constant = Expression.Constant(c.Value);
            state.Constants.Add(constant);
            return state;
        }
    }
}
