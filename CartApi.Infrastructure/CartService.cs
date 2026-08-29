using CartApi.Infrastructure.IRepository;
using CartApi.Models;

namespace CartApi.Infrastructure
{
	public class CartService
	{
		private readonly ICartRepository _repo;

		public CartService(ICartRepository repo)
		{
			_repo = repo;
		}

		public async Task<Cart> GetCart(string userId)
		{
			return await _repo.GetCartAsync(userId) ?? new Cart { UserId = userId };
		}

		public async Task<Cart> AddItem(string userId, CartItem item)
		{
			var cart = await GetCart(userId);

			var existingItem = cart.Items
				.FirstOrDefault(x => x.ProductId == item.ProductId);

			if (existingItem != null)
			{
				existingItem.Quantity += item.Quantity;
			}
			else
			{
				cart.Items.Add(item);
			}

			await _repo.SaveCartAsync(cart);
			return cart;
		}

		public async Task<Cart> RemoveItem(string userId, string productId)
		{
			var cart = await GetCart(userId);

			cart.Items.RemoveAll(x => x.ProductId == productId);

			await _repo.SaveCartAsync(cart);
			return cart;
		}

		public async Task ClearCart(string userId)
		{
			await _repo.DeleteCartAsync(userId);
		}
	}
}
