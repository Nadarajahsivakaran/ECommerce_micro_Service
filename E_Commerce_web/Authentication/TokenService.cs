using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace E_Commerce_Web.Authentication
{
	public class TokenService
	{
		private readonly ProtectedLocalStorage _localStorage;
		private const string TokenKey = "authToken";

		public TokenService(ProtectedLocalStorage localStorage)
		{
			_localStorage = localStorage;
		}

		public async Task<string?> GetAccessTokenAsync()
		{
			var result = await _localStorage.GetAsync<string>(TokenKey);
			return result.Success ? result.Value : null;
		}

		public async Task SetAccessTokenAsync(string token)
		{
			await _localStorage.SetAsync(TokenKey, token);
		}

		public async Task DeleteAccessTokenAsync()
		{
			await _localStorage.DeleteAsync(TokenKey);
		}
	}
}
