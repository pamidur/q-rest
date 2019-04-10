using System;
using System.Collections.Generic;

namespace ODataSamples.Contexts
{
    public static class InMemoryCtx
    {
        public static IEnumerable<Entities.Customer> _customers = new List<Entities.Customer>
        {
            new Entities.Customer
            {
                Id=1,
                FirstName="John",
                LastName="Smith",
                Discount = 1.0M
            },
             new Entities.Customer
            {
                Id=2,
                FirstName="James",
                LastName="Bond",
                Discount = 2.0M
            }
        };

        public static IEnumerable<Entities.Order> _orders = new List<Entities.Order> {
            new Entities.Order
            {
                Id=1,
                OrderDate = new DateTimeOffset(DateTime.Now.AddDays(-2)),
                CustomerId = 1,
                Title = "Candies"
            },
             new Entities.Order
            {
                Id=2,
                OrderDate = new DateTimeOffset(DateTime.Now.AddDays(-52)),
                CustomerId = 1,
                Title = "Pies"
            }
             ,
             new Entities.Order
            {
                Id=3,
                OrderDate = new DateTimeOffset(DateTime.Now.AddDays(-1)),
                CustomerId = 1,
                Title = "Cheese cake"
            },

             new Entities.Order
            {
                Id=4,
                OrderDate = new DateTimeOffset(DateTime.Now.AddDays(-7)),
                CustomerId = 2,
                Title = "Ski"
            },
             new Entities.Order
            {
                Id=5,
                OrderDate = new DateTimeOffset(DateTime.Now),
                CustomerId = 2,
                Title = "Helmet"
            }
        };

       
    }
}
