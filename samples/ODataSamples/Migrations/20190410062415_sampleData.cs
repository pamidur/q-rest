using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ODataSamples.Migrations
{
    public partial class sampleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Discount", "FirstName", "LastName" },
                values: new object[] { 1, 1.0m, "John", "Smith" });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Discount", "FirstName", "LastName" },
                values: new object[] { 2, 2.0m, "James", "Bond" });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CustomerId", "OrderDate", "Title" },
                values: new object[,]
                {
                    { 1, 1, new DateTimeOffset(new DateTime(2019, 4, 8, 9, 24, 15, 684, DateTimeKind.Unspecified).AddTicks(6317), new TimeSpan(0, 3, 0, 0, 0)), "Candies" },
                    { 2, 1, new DateTimeOffset(new DateTime(2019, 2, 17, 9, 24, 15, 684, DateTimeKind.Unspecified).AddTicks(6355), new TimeSpan(0, 2, 0, 0, 0)), "Pies" },
                    { 3, 1, new DateTimeOffset(new DateTime(2019, 4, 9, 9, 24, 15, 684, DateTimeKind.Unspecified).AddTicks(6362), new TimeSpan(0, 3, 0, 0, 0)), "Cheese cake" },
                    { 4, 2, new DateTimeOffset(new DateTime(2019, 4, 3, 9, 24, 15, 684, DateTimeKind.Unspecified).AddTicks(6366), new TimeSpan(0, 3, 0, 0, 0)), "Ski" },
                    { 5, 2, new DateTimeOffset(new DateTime(2019, 4, 10, 9, 24, 15, 684, DateTimeKind.Unspecified).AddTicks(6371), new TimeSpan(0, 3, 0, 0, 0)), "Helmet" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
