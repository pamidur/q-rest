using QRest.Core.Contracts;
using QRest.Core.Expressions;
using QRest.Core.Extensions;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Compiller.Standard
{
    internal class StandardCompilerContext : ICompilationContext
    {
        private readonly bool _finalize;

        public StandardCompilerContext(bool finalize)
        {
            _finalize = finalize;
        }

        public virtual Expression Assemble(ITermSequence sequence, Expression context, ParameterExpression root)
        {
            var exp = sequence.CreateExpression(this, context, root);

            if (_finalize)
                exp = Finalize(exp);

            return exp;
        }

        protected virtual Expression Finalize(Expression exp)
        {
            var eType = exp.GetQueryElementType();

            if (eType == null)
                return exp;

            var name = NamedExpression.DefaultQueryResultName;

            if (exp.NodeType == NamedExpression.NamedExpressionType)
                name = ((NamedExpression)exp).Name;

            exp = new NamedExpression(Expression.Call(typeof(Enumerable), nameof(Enumerable.ToArray), new[] { eType }, exp), name);

            return exp;
        }
    }
}
