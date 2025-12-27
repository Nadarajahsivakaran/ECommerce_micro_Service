using ECommerce.Data;
using ECommerce.Data.Middleware;
using ECommerce.Data.Profiles;
using ECommerce.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.IRepository;
using ProductApi.Infrastructure.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

# region Needed for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Db connection
builder.Services.AddDbContext<ProductDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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
				// Prevent default 401 response
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

#region Dependecy injection -- Services
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
#endregion

#region Register AutoMapper
builder.Services.AddCommonAutoMapper(typeof(ProductProfile).Assembly);
#endregion

var app = builder.Build();


#region Configure the HTTP request pipeline and swagger
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.MapGet("/", () => Results.Redirect("/swagger"));
#endregion

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

#region enable global exception handler
app.UseGlobalExceptionHandler();
#endregion



app.MapControllers();

app.Run();
