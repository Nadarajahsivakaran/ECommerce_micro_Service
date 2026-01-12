public class TokenService
{
	public string? AccessToken { get; private set; }

	public void SetToken(string token)
	{
		AccessToken = token;
	}

	public void ClearToken()
	{
		AccessToken = null;
	}
}
