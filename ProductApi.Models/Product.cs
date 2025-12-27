using ECommerce.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApi.Models
{
	public class Product : BaseEntity
	{
		[Required]
		[MaxLength(150)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(500)]
		public string? Description { get; set; }

		[Required]
		[Range(0.01, 9999999)]
		[Column(TypeName = "decimal(18,2)")]
		public decimal Price { get; set; }

		[Required]
		[Range(0, int.MaxValue)]
		public int Stock { get; set; }

		// Foreign Key
		[Required]
		public Guid CategoryId { get; set; }

		[ForeignKey(nameof(CategoryId))]
		public Category Category { get; set; }
		public bool IsActive { get; set; } = true;
	}
}
