using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Terms;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Assembler
{
    public class ConstantsCollectingVisitor : TermVisitor<List<ConstantExpression>>
    {
        public IReadOnlyList<ConstantExpression> Collect(ITerm lambda)
        {
            var list = Visit(lambda,new List<ConstantExpression>());
            return list.ToArray();
        }

        protected override List<ConstantExpression> VisitConstant(ConstantTerm c, List<ConstantExpression> context)
        {
            var constant = Expression.Constant(c.Value);
            context.Add(constant);
            return context;
        }
    }
}
