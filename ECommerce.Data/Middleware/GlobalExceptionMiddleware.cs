using ECommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;


namespace ECommerce.Data.Middleware
{
	public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
	{
		private readonly RequestDelegate _next = next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled exception occurred.");
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception ex)
		{
			HttpStatusCode statusCode = ex switch
			{
				KeyNotFoundException => HttpStatusCode.NotFound,
				UnauthorizedAccessException => HttpStatusCode.Unauthorized,
				ArgumentException => HttpStatusCode.BadRequest,
				_ => HttpStatusCode.InternalServerError
			};

			ApiResponse<string> response = ApiResponse<string>.FailResponse(ex.Message, "An unexpected error occurred.", (int)statusCode);
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)statusCode;

			string json = JsonSerializer.Serialize(response);
			return context.Response.WriteAsync(json);
		}
	}
}
