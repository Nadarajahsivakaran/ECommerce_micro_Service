using ECommerce.Data;
using ProductApi.Infrastructure.IRepository;
using ProductApi.Models;
namespace ProductApi.Infrastructure.Repository
{
    public class ProductRepository(ProductDbContext context) : GenericRepository<Product>(context), IProductRepository
    {
    }
}
