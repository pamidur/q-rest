using QRest.Core;
using QRest.Core.Contracts;
using QRest.Core.Operations.Boolean;
using QRest.Core.Terms;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace QRest.Semantics.MethodChain.Tests
{
    public class Data
    {
        public int Value { get; set; } = 13;
        public string Text { get; set; } = "Test";
    }

    public class DebugTests
    {
        //[Fact]
        //public void TestDebug()
        //{
        //    var tree = new PropertyTerm
        //    {
        //        PropertyName = nameof(Data.Value),                
        //        Next = new MethodTerm
        //        {
        //            Operation = new EqualOperation(),
        //            Arguments = new List<ITerm>
        //            {
        //                new ConstantTerm
        //                {
        //                    Value = 14
        //                }
        //            }
        //        }
        //    };

        //    var debug = new TermTreeCompiler().CompileDebug<Data>(tree);

        //    var func = debug.Func.Compile();

        //    var result = func(new Data());
        //    var result2 = func(new Data() { Value = 14 });
        //}
    }
}
