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
			var electronicsId = Guid.Parse("11111111-1111-1111-1111-111111111111");
			var clothingId = Guid.Parse("22222222-2222-2222-2222-222222222222");

			var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
				}
			);

			modelBuilder.Entity<Product>().HasData(
				new Product
				{
					Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
					Name = "Wireless Mouse",
					Description = "Ergonomic wireless mouse",
					Price = 19.99m,
					Stock = 150,
					CategoryId = electronicsId,
					IsActive = true,
					ImageUrl = "https://example.com/images/mouse.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				},
				new Product
				{
					Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
					Name = "Cotton T-Shirt",
					Description = "Plain white cotton t-shirt",
					Price = 9.99m,
					Stock = 300,
					CategoryId = clothingId,
					IsActive = true,
					ImageUrl = "https://example.com/images/tshirt.jpg",
					CreatedAt = seedDate,
					UpdatedAt = seedDate
				}
			);
		}
	}
}