using ECommerce.Models;

namespace AuthApi.Models
{
    public class RefreshToken : BaseEntity
	{
		public string Token { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
		public DateTime Expires { get; set; }
		public bool IsRevoked { get; set; } = false;
	}
}
