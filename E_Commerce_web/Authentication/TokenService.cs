//using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

//public class TokenService
//{
//	private readonly ProtectedLocalStorage _storage;

//	public TokenService(ProtectedLocalStorage storage)
//	{
//		_storage = storage;
//	}

//	public async Task SetTokensAsync(string accessToken, string refreshToken)
//	{
//		await _storage.SetAsync("access_token", accessToken);
//		await _storage.SetAsync("refresh_token", refreshToken);
//	}

//	public async Task<string?> GetAccessTokenAsync()
//	{
//		var result = await _storage.GetAsync<string>("access_token");
//		return result.Success ? result.Value : null;
//	}

//	public async Task<string?> GetRefreshTokenAsync()
//	{
//		var result = await _storage.GetAsync<string>("refresh_token");
//		return result.Success ? result.Value : null;
//	}

//	public async Task ClearTokensAsync()
//	{
//		await _storage.DeleteAsync("access_token");
//		await _storage.DeleteAsync("refresh_token");
//	}
//}
