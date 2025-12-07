
using ECommerce.Models;

namespace ECommerce.Data
{
    public interface IGenericRepository<TEntity> where TEntity : class
	{
		Task<IEnumerable<TEntity>> GetAllAsync();
		Task<TEntity?> GetByIdAsync(Guid id);
		Task AddAsync(TEntity entity);
		void Update(TEntity entity);
		void Delete(TEntity entity);
		//Task<bool> SaveChangesAsync();
	}
}
