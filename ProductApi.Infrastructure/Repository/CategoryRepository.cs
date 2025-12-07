using ECommerce.Data;
using ProductApi.Infrastructure.IRepository;
using ProductApi.Models;
namespace ProductApi.Infrastructure.Repository
{
    public class CategoryRepository(ProductDbContext context) : GenericRepository<Category>(context), ICategoryRepository
    {
    }
}
