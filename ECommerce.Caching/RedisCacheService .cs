using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.Caching
{
	public class RedisCacheService : ICacheService
	{
		private readonly IDatabase _db;
		private readonly ILogger<RedisCacheService> _logger;
		private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(10);
		private static readonly JsonSerializerOptions SerializerOptions = new()
		{
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
		{
			_db = redis.GetDatabase();
			_logger = logger;
		}

		public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
		{
			try
			{
				var value = await _db.StringGetAsync(key);
				if (value.IsNullOrEmpty)
					return default;

				return JsonSerializer.Deserialize<T>((string)value!, SerializerOptions);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Redis GET failed for key {Key}. Falling back to source.", key);
				return default;
			}
		}

		public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken ct = default)
		{
			var cached = await GetAsync<T>(key, ct);
			if (cached is not null)
				return cached;

			var value = await factory();

			if (value is not null)
				await SetAsync(key, value, expiry, ct);

			return value;
		}

		public async Task RemoveAsync(string key, CancellationToken ct = default)
		{
			try
			{
				await _db.KeyDeleteAsync(key);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Redis DELETE failed for key {Key}. Cache may be stale until it expires.", key);
			}
		}

		public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
		{
			try
			{
				var json = JsonSerializer.Serialize(value, SerializerOptions);
				await _db.StringSetAsync(key, json, expiry ?? DefaultExpiry);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Redis SET failed for key {Key}. Value was not cached.", key);
			}
		}
	}
}