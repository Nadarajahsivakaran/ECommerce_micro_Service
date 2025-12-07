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

			// Optional: EF Core will usually detect this automatically
			modelBuilder.Entity<Product>()
				.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.CategoryId);

	//		modelBuilder.Entity<Category>().HasData(
	//			new Category
	//			{
	//				Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
	//				Name = "Electronics",
	//				Description = "Electronic devices"
	//			},
	//			new Category
	//			{
	//				Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
	//				Name = "Clothing",
	//				Description = "Fashion items"
	//			}
	//);

	//		modelBuilder.Entity<Product>().HasData(
	//			new Product
	//			{
	//				Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
	//				Name = "Laptop",
	//				Price = 999,
	//				Stock = 10,
	//				CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111")
	//			}
	//		);
		}
	}
}