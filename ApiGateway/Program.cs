using ECommerce.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
	config.ReadFrom.Configuration(context.Configuration);
});

#region JWT Authentication (CENTRALIZED)
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
					"Authentication failed  in api gateway",
					null,
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
					null,
					StatusCodes.Status403Forbidden
				);
				return context.Response.WriteAsJsonAsync(response);
			}
		};
	});
#endregion


builder.Services.AddAuthorizationBuilder()
	.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
	.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"))
	.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("gateway", new OpenApiInfo
	{
		Title = "API Gateway",
		Version = "v1"
	});

	options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
	{
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		Description = "JWT Authorization header using the Bearer scheme."
	});

	options.AddSecurityRequirement(document => new() 
	{ 
		[new OpenApiSecuritySchemeReference("Bearer", document)] = [] 
	});

});

builder.Services
	.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.RoutePrefix = "gateway";
	options.SwaggerEndpoint("/swagger/product/v1/swagger.json", "Product API");
	options.SwaggerEndpoint("/swagger/auth/v1/swagger.json", "Auth API");
	options.EnablePersistAuthorization();
});
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/gateway"));
app.MapReverseProxy();

app.Run();
