using QRest.Core.Contracts;
using QRest.Core.Exceptions;
using QRest.Core.Terms;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard
{
    public abstract class TermVisitor
    {
        //public virtual (LambdaExpression Lambda, IReadOnlyList<Expression> Constants) AssembleSequence(TermSequence s, Expression root, Expression ctx)
        //{
        //    if (s is LambdaSequence lambda)
        //        return AssembleLambdaSequence(lambda, root, ctx);
        //    else return AssembleTermSequence(s, root, ctx);
        //}


        //public virtual (LambdaExpression Lambda, IReadOnlyList<Expression> Constants) AssembleLambdaSequence(LambdaSequence s, Expression root, Expression ctx)
        //{
        //}

        public virtual (LambdaExpression Lambda, IReadOnlyList<ConstantExpression> Constants) AssembleSequence(TermSequence s, Expression root, Expression ctx)
        {
            var rootparam = Expression.Parameter(root.Type, "r");
            var ctxparam = Expression.Parameter(ctx.Type, "c");

            var constants = new List<ConstantExpression>
            {
            };
            var parameters = new List<ParameterExpression> {
                rootparam,
                ctxparam
            };

            var rootexp = s.RootSelector.CreateCallExpression(rootparam, ctxparam, new Expression[] { });

            Expression ctxexp = ctxparam;

            foreach (var term in s)
            {
                var builtTerm = AssembleTerm(term, rootexp, ctxexp);
                constants.AddRange(builtTerm.Constants);
                parameters.AddRange(builtTerm.Parameters);
                ctxexp = builtTerm.Expression;
            }
            return (Expression.Lambda(ctxexp, parameters), constants);
        }


        protected virtual (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleTerm(ITerm term, Expression rootexp, Expression ctxexp)
        {
            var constants = new List<ConstantExpression>();
            var parameters = new List<ParameterExpression>();

            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) result = default;

            switch (term)
            {
                case ConstantTerm c:
                    result = AssembleConstant(c, rootexp, ctxexp);
                    break;
                case PropertyTerm p:
                    result = AssembleProperty(p, rootexp, ctxexp);
                    break;
                case NameTerm n:
                    result = AssembleName(n, rootexp, ctxexp);
                    break;
                case MethodTerm m:
                    result = AssembleMethod(m, rootexp, ctxexp);
                   break;
                default: throw new TermTreeCompilationException($"Unknown Term type '{term.GetType().Name}'");
            }

            ctxexp = result.Expression;
            if (result.Constants != null && result.Constants.Count != 0)
                constants.AddRange(result.Constants);
            if (result.Parameters != null && result.Parameters.Count != 0)
                parameters.AddRange(result.Parameters);

            return (ctxexp, constants, parameters);
        }

        protected abstract (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleName(NameTerm n, Expression root, Expression ctx);
        protected abstract (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleMethod(MethodTerm m, Expression root, Expression ctx);
        protected abstract (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleProperty(PropertyTerm p, Expression root, Expression ctx);
        protected abstract (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleConstant(ConstantTerm c, Expression root, Expression ctx);
    }
}
