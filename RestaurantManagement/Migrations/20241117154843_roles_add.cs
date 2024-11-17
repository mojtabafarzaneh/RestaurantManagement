using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantManagement.Migrations
{
    /// <inheritdoc />
    public partial class roles_add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("3c48e809-d3ed-4730-aa75-88f86773cc8a"), null, "Chef", "CHEF" },
                    { new Guid("711bf6a9-5630-425a-892b-4be2a2041607"), null, "Admin", "ADMIN" },
                    { new Guid("8766e530-18eb-439c-9569-084f859e40e5"), null, "Customer", "CUSTOMER" },
                    { new Guid("ce05782b-e00c-4183-8692-361178857935"), null, "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3c48e809-d3ed-4730-aa75-88f86773cc8a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("711bf6a9-5630-425a-892b-4be2a2041607"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8766e530-18eb-439c-9569-084f859e40e5"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ce05782b-e00c-4183-8692-361178857935"));
        }
    }
}
