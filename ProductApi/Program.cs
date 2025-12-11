using ECommerce.Data;
using ECommerce.Data.Middleware;
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
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Dependecy injection -- Services
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
#endregion

# region Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));
#endregion

var app = builder.Build();

#region enable global exception handler
app.UseGlobalExceptionHandler();
#endregion


#region Configure the HTTP request pipeline and swagger
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.MapGet("/", () => Results.Redirect("/swagger"));
#endregion

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
