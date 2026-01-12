using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductApi.Models.DTO
{
    public class ProductCreateDto
    {
		[Required(ErrorMessage = "Product name is required")]
		[MaxLength(150, ErrorMessage = "Product name cannot exceed 150 characters")]
		public string Name { get; set; } = string.Empty;

		[MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
		public string? Description { get; set; }

		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
		public double Price { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
		public int Stock { get; set; }

		[Required(ErrorMessage = "CategoryId is required")]
		public Guid CategoryId { get; set; }
		public string? ImageUrl { get; set; }
	}
} 
