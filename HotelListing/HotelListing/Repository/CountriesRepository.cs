using AutoMapper;
using HotelListing.Contracts;
using HotelListing.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Repository
{
    public class CountriesRepository : Repository<CountryEntity>, ICountriesRepository
    {
        private readonly DbContext _context;
        public CountriesRepository(DbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public async Task<CountryEntity> GetDetails(int id)
        {
            return await _context.Set<CountryEntity>()
                .Include(q => q.Hotels)
                .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}