using AuthApi.Data;
using AuthApi.Data.IRepository;
using AuthApi.Data.Repository;
using AuthApi.Models;
using ECommerce.Data;
using ECommerce.Data.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ECommerce.Data.Profiles;

var builder = WebApplication.CreateBuilder(args);

#region Needed for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Db connection
builder.Services.AddDbContext<AuthDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	// Allow email addresses as usernames
	options.User.AllowedUserNameCharacters =
		"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = true; 
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

#endregion

#region JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateIssuerSigningKey = true,
		ValidateLifetime = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(key)
	};
});
#endregion

#region Custom services
builder.Services.AddScoped<IAuthService,AuthService>();
#endregion

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

#region Automapper
builder.Services.AddCommonAutoMapper(typeof(AuthProfile).Assembly );
#endregion

var app = builder.Build();

#region enable global exception handler
app.UseGlobalExceptionHandler();
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
// Make Swagger UI default page
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
