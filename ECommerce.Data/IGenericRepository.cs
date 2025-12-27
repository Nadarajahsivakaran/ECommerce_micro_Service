
using ECommerce.Models;
using System.Linq.Expressions;

namespace ECommerce.Data
{
    public interface IGenericRepository<TEntity> where TEntity : class
	{
		Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
			params Expression<Func<TEntity, object>>[] includes);
		Task<TEntity?> GetByIdAsync(Guid id);
		Task AddAsync(TEntity entity);
		Task Update(TEntity entity);
		void Delete(TEntity entity);
		Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>> predicate,
			params Expression<Func<TEntity, object>>[] includes);


	}
}
