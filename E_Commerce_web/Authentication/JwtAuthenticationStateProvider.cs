using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace E_Commerce_Web.Authentication
{
	public class JwtAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly TokenService _tokenService;

		public JwtAuthenticationStateProvider(TokenService tokenService)
		{
			_tokenService = tokenService;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await _tokenService.GetAccessTokenAsync();

			var identity = string.IsNullOrEmpty(token)
				? new ClaimsIdentity()
				: GetClaimsIdentity(token);

			return new AuthenticationState(new ClaimsPrincipal(identity));
		}

		public async Task MarkUserAsAuthenticated(string token)
		{
			await _tokenService.SetAccessTokenAsync(token);

			var identity = GetClaimsIdentity(token);
			var user = new ClaimsPrincipal(identity);

			NotifyAuthenticationStateChanged(
				Task.FromResult(new AuthenticationState(user))
			);
		}

		public async Task MarkUserAsLoggedOut()
		{
			await _tokenService.DeleteAccessTokenAsync();

			var identity = new ClaimsIdentity();
			var user = new ClaimsPrincipal(identity);

			NotifyAuthenticationStateChanged(
				Task.FromResult(new AuthenticationState(user))
			);
		}

		private ClaimsIdentity GetClaimsIdentity(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(token);
			return new ClaimsIdentity(jwtToken.Claims, "jwt");
		}
	}
}
