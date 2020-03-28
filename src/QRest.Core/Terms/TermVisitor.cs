using System;

namespace QRest.Core.Terms
{
    public abstract class TermVisitor<T>
        where T : struct
    {
        protected virtual T Visit(ITerm term, in T state)
        {
            T result;

            switch (term)
            {
                case ConstantTerm c:
                    result = VisitConstant(c, in state);
                    break;
                case PropertyTerm p:
                    result = VisitProperty(p, in state);
                    break;
                case NameTerm n:
                    result = VisitName(n, in state);
                    break;
                case ContextTerm x:
                    result = VisitContext(x, in state);
                    break;
                case MethodTerm m:
                    result = VisitMethod(m, in state);
                    break;
                case LambdaTerm l:
                    result = VisitLambda(l, in state);
                    break;
                case SequenceTerm s:
                    result = VisitSequence(s, in state);
                    break;
                default:
                    result = VisitCustom(term, in state);
                    break;
            }

            return result;
        }

        protected virtual T VisitCustom(ITerm term, in T state)
        {
            throw new InvalidOperationException($"Unknown Term type '{term.GetType().Name}'");
        }

        protected virtual T VisitSequence(SequenceTerm s, in T state)
        {
            var ctx = state;
            foreach (var term in s)
                ctx = Visit(term, in ctx);
            return ctx;
        }

        protected virtual T VisitMethod(MethodTerm m, in T state)
        {
            var ctx = state;
            foreach (var term in m.Arguments)
                ctx = Visit(term, ctx);
            return ctx;
        }

        protected virtual T VisitLambda(LambdaTerm l, in T state) => Visit(l.Term, in state);
        protected virtual T VisitProperty(PropertyTerm p, in T state) => state;
        protected virtual T VisitConstant(ConstantTerm c, in T state) => state;
        protected virtual T VisitName(NameTerm n, in T state) => state;
        protected virtual T VisitContext(ContextTerm x, in T state) => state;
    }
}
