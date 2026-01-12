//using AuthApi.Models.DTO;
//using ECommerce.Models;
//using System.Net;
//using System.Net.Http.Headers;
//using System.Net.Http.Json;

//public class JwtAuthorizationMessageHandler : DelegatingHandler
//{
//	private readonly TokenService _tokenService;
//	private readonly IHttpClientFactory _factory;

//	public JwtAuthorizationMessageHandler(
//		TokenService tokenService,
//		IHttpClientFactory factory)
//	{
//		_tokenService = tokenService;
//		_factory = factory;
//	}

//	protected override async Task<HttpResponseMessage> SendAsync(
//		HttpRequestMessage request,
//		CancellationToken cancellationToken)
//	{
//		var token = await _tokenService.GetAccessTokenAsync();

//		if (!string.IsNullOrWhiteSpace(token))
//		{
//			request.Headers.Authorization =
//				new AuthenticationHeaderValue("Bearer", token);
//		}

//		var response = await base.SendAsync(request, cancellationToken);

//		// 🔁 Access token expired → refresh
//		if (response.StatusCode == HttpStatusCode.Unauthorized)
//		{
//			var refreshToken = await _tokenService.GetRefreshTokenAsync();
//			if (refreshToken == null)
//				return response;

//			var refreshClient = _factory.CreateClient("AuthApi");

//			var refreshResponse = await refreshClient.PostAsJsonAsync(
//				"api/Auth/Refresh",
//				new { RefreshToken = refreshToken });

//			if (!refreshResponse.IsSuccessStatusCode)
//				return response;

//			var result =
//				await refreshResponse.Content.ReadFromJsonAsync<
//					ApiResponse<LoginResponseDto>>();

//			if (result?.Data == null)
//				return response;

//			await _tokenService.SetTokensAsync(
//				result.Data.AccessToken,
//				result.Data.RefreshToken);

//			request.Headers.Authorization =
//				new AuthenticationHeaderValue("Bearer", result.Data.AccessToken);

//			return await base.SendAsync(request, cancellationToken);
//		}

//		return response;
//	}
//}
