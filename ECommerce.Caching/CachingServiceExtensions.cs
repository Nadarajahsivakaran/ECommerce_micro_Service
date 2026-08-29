using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ECommerce.Caching
{
	public static class CachingServiceExtensions
	{
		public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("Redis")
				?? throw new InvalidOperationException("Redis connection string 'Redis' is not configured.");

			services.AddSingleton<IConnectionMultiplexer>(
				ConnectionMultiplexer.Connect(connectionString));

			services.AddSingleton<ICacheService, RedisCacheService>();

			return services;
		}
	}
}
