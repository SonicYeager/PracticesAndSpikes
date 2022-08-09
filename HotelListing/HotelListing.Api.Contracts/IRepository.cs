using HotelListing.Api.Data.Models;

namespace HotelListing.Api.Contracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public Task<TEntity> GetAsync(int? id);
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public Task<QueryResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
        public Task<TEntity> AddAsync(TEntity entity);
        public Task DeleteAsync(int id);
        public Task UpdateAsync(TEntity entity);

        public Task<bool> Exists(int id);
    }
}