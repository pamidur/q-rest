namespace QRest.Core.Operations.SelectContainers
{
    public class SelectContainer { }

    public class SelectContainer01<T01> : SelectContainer
    {
        public T01 F01 { get; set; }
    }

    public class SelectContainer02<T01,T02> : SelectContainer
    {
        public T01 F01 { get; set; }
        public T02 F02 { get; set; }
    }

    public class SelectContainer03<T01, T02, T03> : SelectContainer
    {
        public T01 F01 { get; set; }
        public T02 F02 { get; set; }
        public T03 F03 { get; set; }
    }

    public class SelectContainer04<T01, T02, T03, T04> : SelectContainer
    {
        public T01 F01 { get; set; }
        public T02 F02 { get; set; }
        public T03 F03 { get; set; }
        public T04 F04 { get; set; }
    }

    public class SelectContainer05<T01, T02, T03, T04, T05> : SelectContainer
    {
        public T01 F01 { get; set; }
        public T02 F02 { get; set; }
        public T03 F03 { get; set; }
        public T04 F04 { get; set; }
        public T05 F05 { get; set; }
    }
}
