using Microsoft.AspNetCore.Builder;

namespace ECommerce.Data.Middleware
{
    public static class ExceptionMiddlewareExtensions
    {
		public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
		{
			return app.UseMiddleware<GlobalExceptionMiddleware>();
		}
	}
}
