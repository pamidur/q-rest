using QRest.AspNetCore.OData.Metadata;
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
                ODataModel.New("TestNS")
                .MapSet(typeof(MyEntity),"TestSet","")
                .Schema;

            var sb = new StringBuilder();

            using (var xw = XmlWriter.Create(sb))            
                Microsoft.OData.Edm.Csdl.CsdlWriter.TryWriteCsdl(edm, xw, Microsoft.OData.Edm.Csdl.CsdlTarget.OData, out var errors);


            var xml = sb.ToString();
        }
    }
}
