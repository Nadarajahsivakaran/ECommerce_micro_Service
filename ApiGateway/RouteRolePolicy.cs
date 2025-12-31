namespace ApiGateway
{
	public class RouteRolePolicy
	{
		public static readonly Dictionary<string, string> Policies = new()
		{
			["/api/product/create"] = "UserOnly",
			["/api/product/getall"] = "AdminOnly" // lowercase path
		};
	}
}