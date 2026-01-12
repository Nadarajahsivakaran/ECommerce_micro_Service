
using Microsoft.OpenApi;
using System.Text;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// --------------------------- Swagger Setup (.NET 10 style) ---------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("gateway", new OpenApiInfo
	{
		Title = "API Gateway",
		Version = "v1"
	});

	//// NEW .NET 10 SECURITY DEFINITION
	//options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
	//{
	//	Type = SecuritySchemeType.Http,
	//	Scheme = "bearer",
	//	BearerFormat = "JWT",
	//	Description = "JWT Authorization header using the Bearer scheme."
	//});

	//// NEW .NET 10 SECURITY REQUIREMENT
	//options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
	//{
	//	[new OpenApiSecuritySchemeReference("bearer", document)] = []
	//});
});

// --------------------------- Reverse Proxy Setup ---------------------------
builder.Services
	.AddReverseProxy()
	.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// --------------------------- Middleware ---------------------------
app.UseSwagger();

app.UseSwaggerUI(options =>
{
	options.RoutePrefix = "gateway";

	// Gateway Swagger
	options.SwaggerEndpoint("/swagger/gateway/swagger.json", "API Gateway");

	// Downstream Swagger
	options.SwaggerEndpoint("/swagger/product/v1/swagger.json", "Product API");
	options.SwaggerEndpoint("/swagger/auth/v1/swagger.json", "Auth API");
});

// Redirect root → /gateway
app.MapGet("/", () => Results.Redirect("/gateway"));

// Reverse proxy
app.MapReverseProxy();

app.Run();
