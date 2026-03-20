using AuthApi.Data;
using AuthApi.Data.IRepository;
using AuthApi.Data.Repository;
using AuthApi.Models;
using ECommerce.Data;
using ECommerce.Data.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
	options.User.AllowedUserNameCharacters =
		"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();
#endregion

#region Custom services
builder.Services.AddScoped<IAuthService, AuthService>();
#endregion

builder.Services.AddControllers();
builder.Services.AddOpenApi();

#region Automapper
builder.Services.AddCommonAutoMapper(typeof(AuthProfile).Assembly);
#endregion

var app = builder.Build();

#region enable global exception handler
app.UseGlobalExceptionHandler();
#endregion

// 🔥 Run EF Core migrations + seed admin user
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
	db.Database.Migrate(); // Creates tables in Azure SQL
	await DbInitializer.SeedAsync(scope.ServiceProvider); // Creates admin user
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();


}



Console.WriteLine($"ENV: {app.Environment.EnvironmentName}");
Console.WriteLine($"Connection: {builder.Configuration.GetConnectionString("DefaultConnection")}");

// Make Swagger UI default page
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
