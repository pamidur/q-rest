using QRest.Core;
using QRest.Core.Compilation;
using QRest.Core.Operations;
using QRest.Core.Terms;
using System;
using System.Dynamic;
using System.Linq;
using Xunit;

namespace QRest.Compiler.Standard.Tests
{
    public class CompileOperationsTests
    {
        private readonly TermCompiler _compiler;

        public CompileOperationsTests()
        {
            _compiler = TermCompiler.Default;
        }

        [Fact(DisplayName = "Can compile NEW Operation")]
        public void New()
        {
            var tree = new MethodTerm(OperationsMap.New,
                    new PropertyTerm(nameof(CompileTestClass.IntProperty)),
                    new PropertyTerm(nameof(CompileTestClass.StringProperty)),
                    new SequenceTerm(new PropertyTerm(nameof(CompileTestClass.StringProperty)), new NameTerm("NewName"))
                );

            var data = new CompileTestClass { IntProperty = 1, StringProperty = "MyText", DateTimeProperty = DateTime.Now };
            var compiled = _compiler.Compile<CompileTestClass>(tree);

            dynamic dynamicResult = compiled(data);

            Assert.Equal("MyText", dynamicResult.StringProperty);
            Assert.Equal("MyText", dynamicResult.NewName);
            Assert.Equal(1, dynamicResult.IntProperty);
        }

        [Fact(DisplayName = "Can compile SELECT Operation")]
        public void Select()
        {
            var tree = new MethodTerm(OperationsMap.Map,
                    new LambdaTerm(
                        new MethodTerm(OperationsMap.New,
                            new PropertyTerm(nameof(CompileTestClass.IntProperty)),
                            new PropertyTerm(nameof(CompileTestClass.StringProperty)),
                            new SequenceTerm(new PropertyTerm(nameof(CompileTestClass.StringProperty)), new NameTerm("NewName"))
                    )));

            var data = new[]
            {
                new CompileTestClass { IntProperty = 1, StringProperty = "MyText", DateTimeProperty = DateTime.Now },
                new CompileTestClass { IntProperty = 2, StringProperty = "AnotherText", DateTimeProperty = DateTime.Now },
            }.AsQueryable();

            var compiled = _compiler.Compile<IQueryable<CompileTestClass>>(tree);

            var result = compiled(data);

            Assert.True(result is IQueryable);

            var resultdata = ((IQueryable)result).Cast<object>();

            Assert.Equal(2, resultdata.Count());

            dynamic dynamicResult = resultdata.First();

            Assert.Equal("MyText", dynamicResult.StringProperty);
            Assert.Equal("MyText", dynamicResult.NewName);
            Assert.Equal(1, dynamicResult.IntProperty);
        }

        [Fact(DisplayName = "Can compile Context Operation")]
        public void Context()
        {
            var tree = new SequenceTerm(
                new MethodTerm(OperationsMap.Where,
                   new LambdaTerm(
                       new SequenceTerm(
                           ContextTerm.Root,
                           new PropertyTerm(nameof(CompileTestClass.StringProperty)),
                           new MethodTerm(OperationsMap.Equal, new ConstantTerm("MyText"))
                       )
                   )
                ),
                new MethodTerm(OperationsMap.New, 
                    ContextTerm.Result, 
                    new SequenceTerm(
                        ContextTerm.Result,
                        new MethodTerm(OperationsMap.Count)
                    )
                )
            );

            var data = new[]
            {
                new CompileTestClass { IntProperty = 1, StringProperty = "MyText", DateTimeProperty = DateTime.Now },
                new CompileTestClass { IntProperty = 2, StringProperty = "AnotherText", DateTimeProperty = DateTime.Now },
            }.AsQueryable();

            var compiled = _compiler.Compile<IQueryable<CompileTestClass>>(tree);

            var result = compiled(data);

            dynamic dynamicResult = result;

            Assert.Equal(1, dynamicResult.count);
            Assert.Equal("MyText", Enumerable.First(dynamicResult.data).StringProperty);
        }
    }
}
