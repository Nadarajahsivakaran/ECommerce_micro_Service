using ECommerce.Data;
using ECommerce.Data.Middleware;
using ECommerce.Data.Profiles;
using ECommerce.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.IRepository;
using ProductApi.Infrastructure.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

#region Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
	{
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		Description = "JWT Authorization header using the Bearer scheme."
	});
	options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
	{
		[new OpenApiSecuritySchemeReference("bearer", document)] = []
	});
});
#endregion

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

#region Db connection
builder.Services.AddDbContext<ProductDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		b => b.MigrationsAssembly("ProductApi.Infrastructure")
	)
);
#endregion

#region Dependency Injection

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
#endregion

#region AutoMapper
builder.Services.AddCommonAutoMapper(typeof(ProductProfile).Assembly);
#endregion



var app = builder.Build();

#region Middleware + Swagger
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger"));
#endregion

app.UseHttpsRedirection();

#region Global Exception Handler
app.UseGlobalExceptionHandler();
#endregion

#region Auth
app.UseAuthentication();
app.UseAuthorization();
#endregion

app.MapControllers();

app.Run();
