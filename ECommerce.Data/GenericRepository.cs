using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
			return await _dbSet.ToListAsync();
		}

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
			return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
		}

  //      public async Task<bool> SaveChangesAsync()
  //      {
		//	return await _context.SaveChangesAsync() > 0;
		//}

        public async void Update(TEntity entity)
        {
			entity.UpdatedAt = DateTime.UtcNow;
			_dbSet.Update(entity);
		}
    }
}
