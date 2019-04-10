using System;

namespace ODataSamples.Domain
{
    public class Order
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string CustomerFirstName { get; set; }
        public decimal Discount { get; set; }
    }
}
