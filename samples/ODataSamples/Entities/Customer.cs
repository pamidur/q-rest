using System.Collections.Generic;

namespace ODataSamples.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Discount { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
