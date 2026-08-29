using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Infrastructure
{
	public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
	{
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Product>()
				.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.CategoryId);

			SeedData(modelBuilder);
		}

		private static void SeedData(ModelBuilder modelBuilder)
		{
			// Fixed GUIDs — must stay static, never Guid.NewGuid()
			var electronicsId = Guid.Parse("11111111-1111-1111-1111-111111111111");
			var clothingId = Guid.Parse("22222222-2222-2222-2222-222222222222");
			var homeId = Guid.Parse("33333333-3333-3333-3333-333333333333");
			var booksId = Guid.Parse("44444444-4444-4444-4444-444444444444");

			// Fixed seed date — must stay static, never DateTime.UtcNow
			var seedDate = new DateTime(2026, 8, 28, 0, 0, 0, DateTimeKind.Utc);

			modelBuilder.Entity<Category>().HasData(
				new Category
				{
					Id = electronicsId,
					Name = "Electronics",
					Description = "Phones, laptops, and gadgets",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Category
				{
					Id = clothingId,
					Name = "Clothing",
					Description = "Apparel and accessories",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Category
				{
					Id = homeId,
					Name = "Home & Kitchen",
					Description = "Furniture, appliances, and kitchenware",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Category
				{
					Id = booksId,
					Name = "Books",
					Description = "Fiction, non-fiction, and educational titles",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				}
			);

			modelBuilder.Entity<Product>().HasData(
				// Electronics
				new Product
				{
					Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
					Name = "Wireless Mouse",
					Description = "Ergonomic wireless mouse with USB receiver",
					Price = 19.99m,
					CategoryId = electronicsId,
					IsActive = true,
					ImageUrl = "https://example.com/images/mouse.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Product
				{
					Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
					Name = "Mechanical Keyboard",
					Description = "RGB backlit mechanical keyboard, blue switches",
					Price = 59.99m,
					CategoryId = electronicsId,
					IsActive = true,
					ImageUrl = "https://example.com/images/keyboard.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Product
				{
					Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
					Name = "27-inch 4K Monitor",
					Description = "IPS panel monitor with HDR support",
					Price = 329.99m,
					CategoryId = electronicsId,
					IsActive = true,
					ImageUrl = "https://example.com/images/monitor.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Product
				{
					Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
					Name = "USB-C Hub",
					Description = "7-in-1 USB-C hub with HDMI and SD card reader",
					Price = 34.99m,
					CategoryId = electronicsId,
					IsActive = true,
					ImageUrl = "https://example.com/images/usbhub.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},

				// Clothing
				new Product
				{
					Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
					Name = "Cotton T-Shirt",
					Description = "Plain white cotton t-shirt, unisex fit",
					Price = 9.99m,
					CategoryId = clothingId,
					IsActive = true,
					ImageUrl = "https://example.com/images/tshirt.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Product
				{
					Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"),
					Name = "Denim Jacket",
					Description = "Classic blue denim jacket, mid-weight",
					Price = 49.99m,
					CategoryId = clothingId,
					IsActive = true,
					ImageUrl = "https://example.com/images/jacket.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Product
				{
					Id = Guid.Parse("b3333333-3333-3333-3333-333333333333"),
					Name = "Running Shoes",
					Description = "Lightweight running shoes with cushioned sole",
					Price = 74.99m,
					CategoryId = clothingId,
					IsActive = true,
					ImageUrl = "https://example.com/images/shoes.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},

				// Home & Kitchen
				new Product
				{
					Id = Guid.Parse("c1111111-1111-1111-1111-111111111111"),
					Name = "Stainless Steel Cookware Set",
					Description = "10-piece cookware set, dishwasher safe",
					Price = 129.99m,
					CategoryId = homeId,
					IsActive = true,
					ImageUrl = "https://example.com/images/cookware.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Product
				{
					Id = Guid.Parse("c2222222-2222-2222-2222-222222222222"),
					Name = "Robot Vacuum Cleaner",
					Description = "Smart robot vacuum with app control",
					Price = 199.99m,
					CategoryId = homeId,
					IsActive = true,
					ImageUrl = "https://example.com/images/vacuum.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},

				// Books
				new Product
				{
					Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"),
					Name = "Clean Code",
					Description = "A Handbook of Agile Software Craftsmanship",
					Price = 39.99m,
					CategoryId = booksId,
					IsActive = true,
					ImageUrl = "https://example.com/images/cleancode.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Product
				{
					Id = Guid.Parse("d2222222-2222-2222-2222-222222222222"),
					Name = "The Pragmatic Programmer",
					Description = "Your journey to mastery, 20th anniversary edition",
					Price = 44.99m,
					CategoryId = booksId,
					IsActive = true,
					ImageUrl = "https://example.com/images/pragprog.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				}
			);
		}
	}
}