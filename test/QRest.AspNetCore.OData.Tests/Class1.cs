using System.Text;
using System.Xml;
using Xunit;

namespace QRest.AspNetCore.OData.Tests
{
    internal class MyEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class MetadataBuilderTests
    {
        [Fact]
        public void Can_Map_Simple_Type()
        {
            var edm =
                MetadataBuilder.New("TestNS")
                .Map(typeof(MyEntity))
                .Build();

            var sb = new StringBuilder();

            using (var xw = XmlWriter.Create(sb))            
                Microsoft.OData.Edm.Csdl.CsdlWriter.TryWriteCsdl(edm, xw, Microsoft.OData.Edm.Csdl.CsdlTarget.OData, out var errors);


            var xml = sb.ToString();
        }
    }
}
