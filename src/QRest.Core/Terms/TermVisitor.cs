using System;

namespace QRest.Core.Terms
{
    public abstract class TermVisitor<T>
        where T : struct
    {
        protected virtual T Visit(ITerm term, in T state)
        {
            var result = term switch
            {
                ConstantTerm c => VisitConstant(c, in state),
                PropertyTerm p => VisitProperty(p, in state),
                NameTerm n => VisitName(n, in state),
                ContextTerm x => VisitContext(x, in state),
                MethodTerm m => VisitMethod(m, in state),
                LambdaTerm l => VisitLambda(l, in state),
                SequenceTerm s => VisitSequence(s, in state),
                _ => VisitCustom(term, in state),
            };
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
                ctx = Visit(term, in ctx);
            return ctx;
        }

        protected virtual T VisitLambda(LambdaTerm l, in T state) => Visit(l.Term, in state);
        protected virtual T VisitProperty(PropertyTerm p, in T state) => state;
        protected virtual T VisitConstant(ConstantTerm c, in T state) => state;
        protected virtual T VisitName(NameTerm n, in T state) => state;
        protected virtual T VisitContext(ContextTerm x, in T state) => state;
    }
}
