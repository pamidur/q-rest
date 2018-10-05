using QRest.Core.Contracts;
using QRest.Core.Expressions;
using QRest.Core.Extensions;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Compiler
{
    internal class DefaultCompilerContext : ICompilationContext
    {
        private bool finalize;

        public DefaultCompilerContext(bool finalize)
        {
            this.finalize = finalize;
        }

        public virtual Expression Assemble(ITermSequence sequence, Expression context, ParameterExpression root)
        {
            return sequence.CreateExpression(this, context, root);
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
