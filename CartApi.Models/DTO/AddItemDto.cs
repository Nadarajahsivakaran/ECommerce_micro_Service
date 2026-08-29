namespace CartApi.Models.DTO
{
	public class AddItemDto
	{
		public string UserId { get; set; }
		public string ProductId { get; set; }
		public int Quantity { get; set; }
	}
}
