namespace CartApi.Models
{
	public class Cart
	{
		public string UserId { get; set; }
		public List<CartItem> Items { get; set; } =[];
		public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
	}
}
