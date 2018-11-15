using System;

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
}
