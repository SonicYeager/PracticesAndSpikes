using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Contracts;
using HotelListing.Api.Core.Exceptions;
using HotelListing.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Core.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly IMapper _mapper;

        public Repository(DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TEntity> GetAsync(int? id)
        {
            if (id is null)
            {
                return null;
            }

            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<TResult> GetAsync<TResult>(int? id)
        {
            var result = await _context.Set<TEntity>().FindAsync(id);

            if (result is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id.HasValue ? id : "No key Provided.");
            }

            return _mapper.Map<TResult>(result);

        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>()
        {
            return await _context.Set<TEntity>()
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<QueryResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters)
        {
            var totalSize = await _context.Set<TEntity>().CountAsync();
            var items = await _context.Set<TEntity>()
                .Skip(queryParameters.StartIndex)
                .Take(queryParameters.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return new QueryResult<TResult>
            {
                Items = items,
                PageNumber = queryParameters.PageNumber,
                RecordNumber = queryParameters.PageSize,
                TotalCount = totalSize
            };
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TResult> AddAsync<TSource, TResult>(TSource sourceEntity)
        {
            var entity = _mapper.Map<TEntity>(sourceEntity);

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<TResult>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            if (await Exists(id))
            {
                var entity = await GetAsync(id);
                _context.Set<TEntity>().Remove(entity!);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync<TSource>(int id, TSource sourceEntity)
        {
            var entity = await GetAsync(id);

            if (entity is null)
            {
                throw new NotFoundException(typeof(TEntity).Name, id);
            }

            _mapper.Map(sourceEntity, entity);
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