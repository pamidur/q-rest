using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QRest.Compiler.Standard.Assembler
{
    public class AssemblerContext
    {
        public Expression Context { get; set; }
        public ParameterExpression Root { get; set; }

        public List<ConstantExpression> Constants { get; } = new List<ConstantExpression>();
        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();
    }
}
