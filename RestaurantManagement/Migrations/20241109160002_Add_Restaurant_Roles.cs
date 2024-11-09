using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantManagement.Migrations
{
    /// <inheritdoc />
    public partial class Add_Restaurant_Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("33bbe8c2-7adb-405a-a1f8-708da6c68dc0"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("275af790-a621-4d07-8768-5bb67d9e4834"), null, "Admin", "ADMIN" },
                    { new Guid("738899e4-0474-4354-a452-e59af25642c8"), null, "Manager", "MANAGER" },
                    { new Guid("937f52a1-646d-4c04-801e-ba0f8ad49201"), null, "Chef", "CHEF" },
                    { new Guid("e14484cb-8ce2-468e-b673-8bab49d06340"), null, "Customer", "CUSTOMER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("275af790-a621-4d07-8768-5bb67d9e4834"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("738899e4-0474-4354-a452-e59af25642c8"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("937f52a1-646d-4c04-801e-ba0f8ad49201"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e14484cb-8ce2-468e-b673-8bab49d06340"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("33bbe8c2-7adb-405a-a1f8-708da6c68dc0"), null, "Admin", "ADMIN" });
        }
    }
}
