//using System;
//using System.Text;

//namespace QRest.Core.Terms
//{   
//    public readonly struct SyntaxVisitorContext
//    {
//        public readonly string Render;

//        public SyntaxVisitorContext(string render)
//        {
//            Render = render;
//        }

//        public SyntaxVisitorContext Append(in ReadOnlySpan<char> data) 
//        {
//            return new SyntaxVisitorContext(Render + data)
//        }
//    }

//    public class SyntaxVisitor : TermVisitor<SyntaxVisitorContext>
//    {

//        public string Render(ITerm term)
//        {
//            return string.Create()
//        }

//        protected override SyntaxVisitorContext VisitName(NameTerm n, in SyntaxVisitorContext state)
//        {
//            return state.Append("@").Append(in n.Name);
//        }
//    }
//}
