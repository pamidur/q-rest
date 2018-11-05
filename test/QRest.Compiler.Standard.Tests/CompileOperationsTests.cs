using QRest.Core;
using QRest.Core.Terms;
using System;
using System.Dynamic;
using System.Linq;
using Xunit;

namespace QRest.Compiler.Standard.Tests
{
    public class CompileTestSubClass
    {
        public int Id { get; set; }
    }
    public class CompileTestClass
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public CompileTestSubClass SubProperty { get; set; }
    }

    public class CompileOperationsTests
    {
        private readonly StandardCompiler _compiler;

        public CompileOperationsTests()
        {
            _compiler = new StandardCompiler();
        }

        [Fact(DisplayName = "Can compile NEW Operation")]
        public void New()
        {
            var tree = new LambdaTerm(BuiltIn.Roots.OriginalRoot)
            {
                new MethodTerm(BuiltIn.Operations.New,new[]
                {
                    new SequenceTerm { new PropertyTerm(nameof(CompileTestClass.IntProperty))},
                    new SequenceTerm { new PropertyTerm(nameof(CompileTestClass.StringProperty))},
                    new SequenceTerm { new PropertyTerm(nameof(CompileTestClass.StringProperty)), new NameTerm("NewName")},
                })
            };

            var data = new CompileTestClass { IntProperty = 1, StringProperty = "MyText", DateTimeProperty = DateTime.Now };
            var compiled = _compiler.Compile<CompileTestClass>(tree);

            dynamic dynamicResult = compiled(data);

            Assert.True(dynamicResult is DynamicObject);
            Assert.Equal("MyText", dynamicResult.StringProperty);
            Assert.Equal("MyText", dynamicResult.NewName);
            Assert.Equal(1, dynamicResult.IntProperty);
        }

        [Fact(DisplayName = "Can compile SELECT Operation")]
        public void Select()
        {
            var tree = new LambdaTerm(BuiltIn.Roots.OriginalRoot)
            {
                new MethodTerm(BuiltIn.Operations.Select, new[]{
                    new LambdaTerm(BuiltIn.Roots.ContextElement){
                        new MethodTerm(BuiltIn.Operations.New,new[]
                        {
                            new SequenceTerm { new PropertyTerm(nameof(CompileTestClass.IntProperty))},
                            new SequenceTerm { new PropertyTerm(nameof(CompileTestClass.StringProperty))},
                            new SequenceTerm { new PropertyTerm(nameof(CompileTestClass.StringProperty)), new NameTerm("NewName")},
                        })
                    }
                })
            };

            var data = new[]
            {
                new CompileTestClass { IntProperty = 1, StringProperty = "MyText", DateTimeProperty = DateTime.Now },
                new CompileTestClass { IntProperty = 2, StringProperty = "AnotherText", DateTimeProperty = DateTime.Now },
            }.AsQueryable();

            var compiled = _compiler.Compile<IQueryable<CompileTestClass>>(tree);

            var result = compiled(data);

            Assert.True(result is IQueryable<DynamicObject>);

            var resultdata = (IQueryable<DynamicObject>)result;

            Assert.Equal(2, resultdata.Count());

            dynamic dynamicResult = resultdata.First();

            Assert.Equal("MyText", dynamicResult.StringProperty);
            Assert.Equal("MyText", dynamicResult.NewName);
            Assert.Equal(1, dynamicResult.IntProperty);
        }
    }
}
