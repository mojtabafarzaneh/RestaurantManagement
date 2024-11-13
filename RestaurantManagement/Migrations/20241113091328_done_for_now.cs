using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantManagement.Migrations
{
    /// <inheritdoc />
    public partial class done_for_now : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: new Guid("70c5b608-0597-4b02-b517-8523f25179ae"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "Available", "Category", "Description", "EstimatedPrepTime", "Name", "Price", "QuantityAvailable" },
                values: new object[] { new Guid("70c5b608-0597-4b02-b517-8523f25179ae"), true, 3, "Ittalian Pizza", 10, "Pizza", 9m, 5 });
        }
    }
}
