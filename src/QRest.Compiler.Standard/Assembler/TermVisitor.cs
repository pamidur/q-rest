using QRest.Core.Contracts;
using QRest.Core.Exceptions;
using QRest.Core.Terms;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Compiler.Standard.Assembler
{
    public abstract class TermVisitor
    {
        protected virtual (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleTerm(ITerm term, ParameterExpression root, Expression ctx)
        {
            var constants = new List<ConstantExpression>();
            var parameters = new List<ParameterExpression>();

            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) result = default;

            switch (term)
            {
                case ConstantTerm c:
                    result = AssembleConstant(c, root, ctx);
                    break;
                case PropertyTerm p:
                    result = AssembleProperty(p, root, ctx);
                    break;
                case NameTerm n:
                    result = AssembleName(n, root, ctx);
                    break;
                case MethodTerm m:
                    result = AssembleMethod(m, root, ctx);
                   break;
                case LambdaTerm l:
                    result = AssembleLambda(l, root, ctx);
                    break;
                case SequenceTerm s:
                    result = AssembleSequence(s, root, ctx);
                    break;
                default:
                    result = ProcessCustomTerm(term, root, ctx);
                    break;
            }

            ctx = result.Expression;
            if (result.Constants != null && result.Constants.Count != 0)
                constants.AddRange(result.Constants);
            if (result.Parameters != null && result.Parameters.Count != 0)
                parameters.AddRange(result.Parameters);

            return (ctx, constants, parameters);
        }

        protected virtual
            (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters)
            ProcessCustomTerm(ITerm term, ParameterExpression root, Expression context)
        {
            throw new TermTreeCompilationException($"Unknown Term type '{term.GetType().Name}'");
        }

        protected virtual (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleSequence(SequenceTerm s, ParameterExpression root, Expression ctx)
        {
            var constants = new List<ConstantExpression>();
            var parameters = new List<ParameterExpression>();

            foreach (var term in s)
            {
                //TODO:: Check const and lambda only in the beginning
                var builtTerm = AssembleTerm(term, root, ctx);
                constants.AddRange(builtTerm.Constants);
                parameters.AddRange(builtTerm.Parameters);
                ctx = builtTerm.Expression;
            }

            return (ctx, constants, parameters);
        }

        protected abstract (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleName(NameTerm n, ParameterExpression root, Expression ctx);
        protected abstract (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleLambda(LambdaTerm l, ParameterExpression root, Expression ctx);
        protected abstract (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleMethod(MethodTerm m, ParameterExpression root, Expression ctx);
        protected abstract (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleProperty(PropertyTerm p, ParameterExpression root, Expression ctx);
        protected abstract (Expression Expression, IReadOnlyList<ConstantExpression> Constants, IReadOnlyList<ParameterExpression> Parameters) AssembleConstant(ConstantTerm c, ParameterExpression root, Expression ctx);
    }
}
