using CartApi.Models;

namespace CartApi.Infrastructure.IRepository
{
	public interface ICartRepository
	{
		Task<Cart?> GetCartAsync(string userId);
		Task SaveCartAsync(Cart cart);
		Task DeleteCartAsync(string userId);
	}
}
