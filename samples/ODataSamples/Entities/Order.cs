using System;

namespace ODataSamples.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
