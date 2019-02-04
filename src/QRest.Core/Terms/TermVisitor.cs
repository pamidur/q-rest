using System;

namespace QRest.Core.Terms
{
    public abstract class TermVisitor<T>
    {
        protected virtual T Visit(ITerm term, T context)
        {
            T result;

            switch (term)
            {
                case ConstantTerm c:
                    result = VisitConstant(c, context);
                    break;
                case PropertyTerm p:
                    result = VisitProperty(p, context);
                    break;
                case NameTerm n:
                    result = VisitName(n, context);
                    break;
                case MethodTerm m:
                    result = VisitMethod(m, context);
                    break;
                case LambdaTerm l:
                    result = VisitLambda(l, context);
                    break;
                case SequenceTerm s:
                    result = VisitSequence(s, context);
                    break;
                default:
                    result = VisitCustom(term, context);
                    break;
            }

            return result;
        }

        protected virtual T VisitCustom(ITerm term, T context)
        {
            throw new InvalidOperationException($"Unknown Term type '{term.GetType().Name}'");
        }

        protected virtual T VisitSequence(SequenceTerm s, T context)
        {
            foreach (var term in s)           
                context = Visit(term, context);  
            return context;
        }

        protected virtual T VisitMethod(MethodTerm m, T context)
        {
            foreach (var term in m.Arguments)
                context = Visit(term, context);
            return context;
        }

        protected virtual T VisitLambda(LambdaTerm l, T context) => Visit(l.Term, context);
        protected virtual T VisitProperty(PropertyTerm p, T context) => context;
        protected virtual T VisitConstant(ConstantTerm c, T context) => context;
        protected virtual T VisitName(NameTerm n, T context) => context;
    }
}
