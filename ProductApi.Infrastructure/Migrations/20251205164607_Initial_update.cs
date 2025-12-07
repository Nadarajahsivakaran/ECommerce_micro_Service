using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 12, 5, 16, 4, 41, 106, DateTimeKind.Utc).AddTicks(2935), "Electronic devices", "Electronics", new DateTime(2025, 12, 5, 16, 4, 41, 106, DateTimeKind.Utc).AddTicks(2941) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 12, 5, 16, 4, 41, 106, DateTimeKind.Utc).AddTicks(4386), "Fashion items", "Clothing", new DateTime(2025, 12, 5, 16, 4, 41, 106, DateTimeKind.Utc).AddTicks(4386) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsActive", "Name", "Price", "Stock", "UpdatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 12, 5, 16, 4, 41, 107, DateTimeKind.Utc).AddTicks(1250), null, true, "Laptop", 999m, 10, new DateTime(2025, 12, 5, 16, 4, 41, 107, DateTimeKind.Utc).AddTicks(1252) });
        }
    }
}
