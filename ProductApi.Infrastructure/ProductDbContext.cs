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
		}
	}
}