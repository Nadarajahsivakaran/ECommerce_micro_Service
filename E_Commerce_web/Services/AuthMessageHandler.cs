using System.Net.Http.Headers;

public class AuthMessageHandler : DelegatingHandler
{
	private readonly TokenService _tokenService;

	public AuthMessageHandler(TokenService tokenService)
	{
		_tokenService = tokenService;
	}

	protected override Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		if (!string.IsNullOrEmpty(_tokenService.AccessToken))
		{
			request.Headers.Authorization =
				new AuthenticationHeaderValue(
					"Bearer",
					_tokenService.AccessToken);
		}

		return base.SendAsync(request, cancellationToken);
	}
}
