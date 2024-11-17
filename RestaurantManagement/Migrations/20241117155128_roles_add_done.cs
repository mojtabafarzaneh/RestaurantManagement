using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantManagement.Migrations
{
    /// <inheritdoc />
    public partial class roles_add_done : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0c6c3a50-d184-40f5-b362-8a4b220848bc"), null, "Chef", "CHEF" },
                    { new Guid("26538b95-cf4c-41c1-83a9-cac2e7c02ec3"), null, "Admin", "ADMIN" },
                    { new Guid("c9fc3f07-a143-48ea-b189-aab63b71280c"), null, "Manager", "MANAGER" },
                    { new Guid("fb31a523-b286-483b-96eb-0a295a2ce1f7"), null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0c6c3a50-d184-40f5-b362-8a4b220848bc"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("26538b95-cf4c-41c1-83a9-cac2e7c02ec3"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c9fc3f07-a143-48ea-b189-aab63b71280c"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fb31a523-b286-483b-96eb-0a295a2ce1f7"));

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
    }
}
