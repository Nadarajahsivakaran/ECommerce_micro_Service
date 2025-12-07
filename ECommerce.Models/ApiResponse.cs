
namespace ECommerce.Models
{
	public class ApiResponse<T>
	{
		public bool Success { get; set; }
		public string? Message { get; set; }
		public T? Data { get; set; }
		public string? Error { get; set; }
		public int StatusCode { get; set; }

		public static ApiResponse<T> SuccessResponse(T data, string? message = null, int statusCode = 200)
		{
			return new ApiResponse<T>
			{
				Success = true,
				Message = message,
				Data = data,
				StatusCode = statusCode
			};
		}

		public static ApiResponse<T> FailResponse(string error, string? message = null, int statusCode = 400)
		{
			return new ApiResponse<T>
			{
				Success = false,
				Message = message,
				Error = error,
				StatusCode = statusCode
			};
		}
	}
}
