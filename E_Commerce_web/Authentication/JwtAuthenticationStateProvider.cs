//using Microsoft.AspNetCore.Components.Authorization;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;

//public class JwtAuthenticationStateProvider : AuthenticationStateProvider
//{
//	private readonly TokenService _tokenService;
//	private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

//	public JwtAuthenticationStateProvider(TokenService tokenService)
//	{
//		_tokenService = tokenService;
//	}

//	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//	{
//		var token = await _tokenService.GetAccessTokenAsync();

//		if (string.IsNullOrWhiteSpace(token))
//			return new AuthenticationState(_anonymous);

//		var claims = new JwtSecurityTokenHandler()
//			.ReadJwtToken(token)
//			.Claims;

//		var identity = new ClaimsIdentity(claims, "jwt");
//		return new AuthenticationState(new ClaimsPrincipal(identity));
//	}

//	public async Task MarkUserAsAuthenticated(string accessToken, string refreshToken)
//	{
//		await _tokenService.SetTokensAsync(accessToken, refreshToken);
//		NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
//	}

//	public async Task Logout()
//	{
//		await _tokenService.ClearTokensAsync();
//		NotifyAuthenticationStateChanged(
//			Task.FromResult(new AuthenticationState(_anonymous)));
//	}
//}
