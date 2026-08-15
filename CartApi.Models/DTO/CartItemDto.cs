namespace CartApi.Models.DTO
{
	public class CartItemDto
	{
		public string ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
	}
}
