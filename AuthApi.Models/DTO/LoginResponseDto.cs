

namespace AuthApi.Models.DTO
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

		public string RefreshToken { get; set; } = string.Empty;

		public IList<string> Roles { get; set; } = [];
	}
}
