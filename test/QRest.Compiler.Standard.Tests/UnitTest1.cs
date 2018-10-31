using QRest.Core.Operations;
using QRest.Core.Operations.Boolean;
using QRest.Core.Operations.Query;
using QRest.Core.Terms;
using System.Linq;
using Xunit;

namespace QRest.Compiler.Standard.Tests
{
    public class TestEntity
    {
        public int IntProperty { get; set; }
    }

    public class CompilationTests
    {
        private readonly StandardCompiler _compiler;

        public CompilationTests()
        {
            _compiler = new StandardCompiler();
        }

        [Fact]
        public void Can_Compile_Simple_Method()
        {
            var seq = new TermSequence {
                new MethodTerm(
                    new EqualOperation(),
                    new[] {
                        new TermSequence { new ConstantTerm("Ololo") }
                    })
            };

            var result = _compiler.Assemble<string, string>(seq, false);
            var compiled = result.Compile();

            Assert.True((bool)compiled("Ololo", "Ololo"));
        }

        [Fact]
        public void Can_Compile_Lambda()
        {
            var seq = new TermSequence {
                new MethodTerm(
                    new WhereOperation(),
                    new[] {
                        new LambdaSequence {
                            new MethodTerm(new ItOperation()),
                            new PropertyTerm(nameof(TestEntity.IntProperty)),
                            new MethodTerm(new EqualOperation(), new[]{ new TermSequence { new ConstantTerm(1) } })
                        }
                    })
            };

            var result = _compiler.Assemble<IQueryable<TestEntity>>(seq, false);
            var compiled = result.Compile();
        }
    }
}
