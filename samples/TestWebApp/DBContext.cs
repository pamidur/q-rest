using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ODataSamples.Contexts
{

    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Discount { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }


    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().HasMany(c => c.Orders);

            modelBuilder.Entity<Customer>().HasData(
                 new Customer
                 {
                     Id = 1,
                     FirstName = "John",
                     LastName = "Smith",
                     Discount = 1.0M
                 },
                 new Customer
                 {
                     Id = 2,
                     FirstName = "James",
                     LastName = "Bond",
                     Discount = 2.0M
                 });

            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    OrderDate = new DateTimeOffset(DateTime.Now.AddDays(-2)),
                    CustomerId = 1,
                    Title = "Candies"
                },
                new Order
                {
                    Id = 2,
                    OrderDate = new DateTimeOffset(DateTime.Now.AddDays(-52)),
                    CustomerId = 1,
                    Title = "Pies"
                }
                ,
                new Order
                {
                    Id = 3,
                    OrderDate = new DateTimeOffset(DateTime.Now.AddDays(-1)),
                    CustomerId = 1,
                    Title = "Cheese cake"
                },

                new Order
                {
                    Id = 4,
                    OrderDate = new DateTimeOffset(DateTime.Now.AddDays(-7)),
                    CustomerId = 2,
                    Title = "Ski"
                },
                new Order
                {
                    Id = 5,
                    OrderDate = new DateTimeOffset(DateTime.Now),
                    CustomerId = 2,
                    Title = "Helmet"
                });
        }
    }
}
