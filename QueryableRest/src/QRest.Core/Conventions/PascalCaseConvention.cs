using QRest.Core.Contracts;

namespace QRest.Core.Conventions
{
    public class PascalCaseConvention : INameConvention
    {
        public string DtoToModel(string name)
        {
            return ToPascal(name);
        }

        public string ModelToDto(string name)
        {
            return ToPascal(name);
        }

        private string ToPascal(string name)
        {
            return $"name[0].ToString().ToUpperInvariant(){name.Substring(1)}";
        }
    }
}
