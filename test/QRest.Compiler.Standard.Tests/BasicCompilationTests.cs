using QRest.Core;
using QRest.Core.Compilation;
using QRest.Core.Operations;
using QRest.Core.Terms;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace QRest.Compiler.Standard.Tests
{
    public class TestEntity
    {
        public int IntProperty { get; set; }
    }

    public class BasicCompilationTests
    {
        private readonly TermCompiler _compiler;

        public BasicCompilationTests()
        {
            _compiler = TermCompiler.Default;
        }

        [Fact]
        public void Can_Compile_Simple_Method()
        {
            var seq =
                new MethodTerm(OperationsMap.Equal,
                    new[] {
                        new ConstantTerm("Ololo")
                    });

            var result = _compiler.Assemble<string>(seq);
            var compiled = result.Compile();

            Assert.True((bool)compiled("Ololo"));
        }

        [Fact]
        public void Can_Compile_Lambda()
        {
            var seq =
                new MethodTerm(OperationsMap.Where,
                    new[] {
                        new LambdaTerm(new SequenceTerm(
                            ContextTerm.Root,
                            new PropertyTerm(nameof(TestEntity.IntProperty)),
                            new MethodTerm(OperationsMap.Equal, new[]{ new ConstantTerm(1) })
                            )
                        )
                    });

            var result = _compiler.Assemble<IQueryable<TestEntity>>(seq);
            var compiled = result.Compile();

            var executed = (IQueryable<TestEntity>)compiled(new List<TestEntity> { new TestEntity { IntProperty = 1 }, new TestEntity { IntProperty = 2 } }.AsQueryable());

            Assert.Contains(executed, e => e.IntProperty == 1);
            Assert.Single(executed);
        }
    }
}
