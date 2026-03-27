using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Data
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
		protected readonly DbContext _context;
		protected readonly DbSet<TEntity> _dbSet;

		public GenericRepository(DbContext context)
		{
			_context = context;
			_dbSet = _context.Set<TEntity>();
		}

		public async Task AddAsync(TEntity entity)
        {
			entity.CreatedAt = DateTime.UtcNow;
			await _dbSet.AddAsync(entity);
			await _context.SaveChangesAsync();
		}

        public void Delete(TEntity entity)
        {
			_dbSet.Remove(entity);
		}
		
        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
			return await _dbSet.FindAsync(id);
		}

        public async Task Update(TEntity entity)
        {
			entity.UpdatedAt = DateTime.UtcNow;
			_dbSet.Update(entity);
			await _context.SaveChangesAsync();
		}

		// Flexible FindAsync
		public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate,
			params Expression<Func<TEntity, object>>[] includes)
		{
			IQueryable<TEntity> query = _context.Set<TEntity>();

			// Apply includes
			if (includes != null && includes.Length > 0)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			// Apply predicate if provided
			if (predicate != null)
				query = query.Where(predicate);

			return await query.ToListAsync();
		}

		// Optional: Find a single entity
		public async Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>> predicate,
			params Expression<Func<TEntity, object>>[] includes)
		{
			IQueryable<TEntity> query = _context.Set<TEntity>();
			if (includes != null)
			{
				foreach (var include in includes)
					query = query.Include(include);
			}
			return await query.FirstOrDefaultAsync(predicate);
		}
	}
}
