using CartApi.Infrastructure.IRepository;
using CartApi.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace CartApi.Infrastructure.Repository
{
	public class CartRepository : ICartRepository
	{
		private readonly IDatabase _db;

		public CartRepository(IConnectionMultiplexer redis)
		{
			_db = redis.GetDatabase();
		}

		private string Key(string userId) => $"cart:{userId}";

		public async Task<Cart?> GetCartAsync(string userId)
		{
			var data = await _db.StringGetAsync(Key(userId));

			if (data.IsNullOrEmpty)
				return null;

			return JsonSerializer.Deserialize<Cart>((string)data!);
		}

		public async Task SaveCartAsync(Cart cart)
		{
			var json = JsonSerializer.Serialize(cart);
			await _db.StringSetAsync(Key(cart.UserId), json, TimeSpan.FromDays(7));
		}

		public async Task DeleteCartAsync(string userId)
		{
			await _db.KeyDeleteAsync(Key(userId));
		}
	}
}
