using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Phones, laptops, and gadgets", "Electronics", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Apparel and accessories", "Clothing", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ergonomic wireless mouse", "https://example.com/images/mouse.jpg", true, "Wireless Mouse", 19.99m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Plain white cotton t-shirt", "https://example.com/images/tshirt.jpg", true, "Cotton T-Shirt", 9.99m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
