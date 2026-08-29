using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.Data
{
	public static class AutoMapperServiceExtensions
	{
		public static IServiceCollection AddCommonAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
		{
			services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));
			return services;
		}
	}
}
