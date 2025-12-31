using ApiGateway;
using AuthApi.Models.DTO;
using ECommerce.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(key)
		};

		options.Events = new JwtBearerEvents
		{
			OnChallenge = context =>
			{
				context.HandleResponse(); 
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				context.Response.ContentType = "application/json";

				var response = ApiResponse<string>.FailResponse(
					"Unauthorized",
					"Authentication failed",
					StatusCodes.Status401Unauthorized
				);

				return context.Response.WriteAsJsonAsync(response);
			},
			OnForbidden = context =>
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				context.Response.ContentType = "application/json";

				var response = ApiResponse<string>.FailResponse(
					"Forbidden",
					"You do not have permission to access this resource",
					StatusCodes.Status403Forbidden
				);
				return context.Response.WriteAsJsonAsync(response);
			}
		};
	});
#endregion

#region Authorization Policies
static string RoleName(UserRole role) => role.ToString();

builder.Services.AddAuthorizationBuilder()
	.AddPolicy("UserOnly", policy => policy.RequireRole(RoleName(UserRole.User)))
	.AddPolicy("AdminOnly", policy => policy.RequireRole(RoleName(UserRole.Admin)))
	.AddPolicy("AdminOrSuperAdmin", policy => policy.RequireRole(RoleName(UserRole.Admin), RoleName(UserRole.SuperAdmin)))
	.AddPolicy("AnyRole", policy => policy.RequireRole(RoleName(UserRole.User), RoleName(UserRole.Admin), RoleName(UserRole.SuperAdmin)));
#endregion

// Add Reverse Proxy
builder.Services.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

#region Middleware Order
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region Reverse Proxy with Dictionary-based Role Enforcement
app.MapReverseProxy(proxyPipeline =>
{
	proxyPipeline.Use(async (context, next) =>
	{
		var path = context.Request.Path.Value?.ToLower() ?? "";

		if (RouteRolePolicy.Policies.TryGetValue(path, out var policyName))
		{
			var authService = context.Request.HttpContext.RequestServices
				.GetRequiredService<IAuthorizationService>();

			var authResult = await authService.AuthorizeAsync(context.User, null, policyName);

			if (!authResult.Succeeded)
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				context.Response.ContentType = "application/json";

				ApiResponse<string> response = ApiResponse<string>.FailResponse(
					"Forbidden",
					"You do not have permission to access this resource",
					StatusCodes.Status403Forbidden
				);

				await context.Response.WriteAsJsonAsync(response);
				return;
			}
		}

		await next();
	});
});
#endregion


app.Run();