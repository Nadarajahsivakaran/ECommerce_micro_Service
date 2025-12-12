using ECommerce.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models
{
    public class Category : BaseEntity
	{
		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(250)]
		public string Description { get; set; } = string.Empty;

		// Navigation property
		public ICollection<Product> Products { get; set; } = [];
	}
}
