using HotelListing.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }

        public async Task<TEntity> GetAsync(int? id)
        {
            if (id is null)
            {
                return null;
            }

            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            if (await Exists(id))
            {
                var entity = await GetAsync(id);
                _context.Set<TEntity>().Remove(entity!);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }
    }
}