using Microsoft.EntityFrameworkCore;
using ODataSamples.Entities;
using System;

namespace ODataSamples.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
