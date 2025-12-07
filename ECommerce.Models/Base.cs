using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
	public abstract class BaseEntity
	{
		[Key]
		public Guid Id { get; set; }       // or int, but Guid is nice for microservices

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
	}
}
