

namespace ProductApi.Models.DTO
{
    public class ProductDto
    {
		public Guid Id { get; set; }           // Product ID
		public string Name { get; set; } = string.Empty;    // Product name
		public string Description { get; set; } = string.Empty;  // Product description
		public double Price { get; set; }      // Product price
		public int Stock { get; set; }         // Available stock
		public bool IsActive { get; set; }     // Product status
		public string CategoryName { get; set; } = string.Empty; // Related category name
	}
}
