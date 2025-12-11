
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.DTO
{
    public class RegisterResponseDto
    {
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		public string Email { get; set; } = string.Empty;
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserId { get; set; }
	}
}
