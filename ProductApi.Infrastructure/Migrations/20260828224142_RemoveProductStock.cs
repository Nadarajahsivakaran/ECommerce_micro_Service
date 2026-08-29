using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Furniture, appliances, and kitchenware", "Home & Kitchen", new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Fiction, non-fiction, and educational titles", "Books", new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Ergonomic wireless mouse with USB receiver", "https://example.com/images/mouse.jpg", true, "Wireless Mouse", 19.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a2222222-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "RGB backlit mechanical keyboard, blue switches", "https://example.com/images/keyboard.jpg", true, "Mechanical Keyboard", 59.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a3333333-3333-3333-3333-333333333333"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "IPS panel monitor with HDR support", "https://example.com/images/monitor.jpg", true, "27-inch 4K Monitor", 329.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a4444444-4444-4444-4444-444444444444"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "7-in-1 USB-C hub with HDMI and SD card reader", "https://example.com/images/usbhub.jpg", true, "USB-C Hub", 34.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b1111111-1111-1111-1111-111111111111"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Plain white cotton t-shirt, unisex fit", "https://example.com/images/tshirt.jpg", true, "Cotton T-Shirt", 9.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b2222222-2222-2222-2222-222222222222"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Classic blue denim jacket, mid-weight", "https://example.com/images/jacket.jpg", true, "Denim Jacket", 49.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b3333333-3333-3333-3333-333333333333"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Lightweight running shoes with cushioned sole", "https://example.com/images/shoes.jpg", true, "Running Shoes", 74.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c1111111-1111-1111-1111-111111111111"), new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "10-piece cookware set, dishwasher safe", "https://example.com/images/cookware.jpg", true, "Stainless Steel Cookware Set", 129.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c2222222-2222-2222-2222-222222222222"), new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Smart robot vacuum with app control", "https://example.com/images/vacuum.jpg", true, "Robot Vacuum Cleaner", 199.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d1111111-1111-1111-1111-111111111111"), new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "A Handbook of Agile Software Craftsmanship", "https://example.com/images/cleancode.jpg", true, "Clean Code", 39.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d2222222-2222-2222-2222-222222222222"), new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Your journey to mastery, 20th anniversary edition", "https://example.com/images/pragprog.jpg", true, "The Pragmatic Programmer", 44.99m, new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a3333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a4444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b3333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("c1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("c2222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d2222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ergonomic wireless mouse", "https://example.com/images/mouse.jpg", true, "Wireless Mouse", 19.99m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Plain white cotton t-shirt", "https://example.com/images/tshirt.jpg", true, "Cotton T-Shirt", 9.99m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }
    }
}
