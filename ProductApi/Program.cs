using ECommerce.Data;
using ECommerce.Data.Middleware;
using ECommerce.Data.Profiles;
using Microsoft.EntityFrameworkCore;
using ProductApi.Infrastructure;
using ProductApi.Infrastructure.IRepository;
using ProductApi.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

# region Needed for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Db connection
builder.Services.AddDbContext<ProductDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		b => b.MigrationsAssembly("ProductApi.Infrastructure")
	)
);
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

#region enable global exception handler
app.UseGlobalExceptionHandler();
#endregion

app.MapControllers();

app.Run();
