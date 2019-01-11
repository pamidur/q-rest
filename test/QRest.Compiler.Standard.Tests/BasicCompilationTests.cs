using QRest.Core;
using QRest.Core.Operations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
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
        private readonly StandardCompiler _compiler;

        public BasicCompilationTests()
        {
            _compiler = new StandardCompiler();
        }

        [Fact]
        public void Can_Compile_Simple_Method()
        {
            var seq = new RootTerm(
                new MethodTerm(
                    new EqualOperation(),
                    new[] {
                        new ConstantTerm("Ololo").AsSequence()
                    })
            );

            var result = _compiler.Assemble<string>(seq);
            var compiled = result.Compile();

            Assert.True((bool)compiled("Ololo"));
        }

        [Fact]
        public void Can_Compile_Lambda()
        {
            var seq = new RootTerm(new[]{
                new MethodTerm(
                    new WhereOperation(),
                    new[] {
                        new LambdaTerm(
                            new MethodTerm(new RootOperation()),
                            new PropertyTerm(nameof(TestEntity.IntProperty)),
                            new MethodTerm(new EqualOperation(), new[]{ new ConstantTerm(1).AsSequence() })
                        )
                    })
            });

            var result = _compiler.Assemble<IQueryable<TestEntity>>(seq);
            var compiled = result.Compile();

            var executed = (IQueryable<TestEntity>)compiled(new List<TestEntity> { new TestEntity { IntProperty = 1 }, new TestEntity { IntProperty = 2 } }.AsQueryable());

            Assert.Contains(executed, e => e.IntProperty == 1);
            Assert.Single(executed);
        }
    }
}
