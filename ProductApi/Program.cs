using ECommerce.Caching;
using ECommerce.Data;
using ECommerce.Data.Middleware;
using ECommerce.Data.Profiles;
using Microsoft.EntityFrameworkCore;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.IRepository;
using ProductApi.Infrastructure.Repository;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Db connection
builder.Services.AddDbContext<ProductDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		b =>
		{
			b.MigrationsAssembly("ProductApi.Infrastructure");
			b.EnableRetryOnFailure(
				maxRetryCount: 10,
				maxRetryDelay: TimeSpan.FromSeconds(10),
				errorNumbersToAdd: null
			);
		}
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

builder.Services.AddRedisCaching(builder.Configuration);


var app = builder.Build();

#region Middleware + Swagger
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	//app.MapOpenApi();
	//app.MapScalarApiReference();
}

app.MapGet("/", () => Results.Redirect("/swagger"));
#endregion

app.UseHttpsRedirection();

#region Global Exception Handler
app.UseGlobalExceptionHandler();
#endregion

app.MapControllers();

app.Run();
