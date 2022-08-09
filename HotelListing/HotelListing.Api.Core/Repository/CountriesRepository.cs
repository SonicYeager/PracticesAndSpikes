using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Contracts;
using HotelListing.Api.Core.Exceptions;
using HotelListing.Api.Data.Entities;
using HotelListing.Api.Data.Models.Country;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Core.Repository
{
    public class CountriesRepository : Repository<CountryEntity>, ICountriesRepository
    {
        private readonly DbContext _context;
        private readonly IMapper _mapper;
        public CountriesRepository(DbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CountryDto> GetDetails(int id)
        {
            var country =  await _context.Set<CountryEntity>()
                .Include(q => q.Hotels)
                .ProjectTo<CountryDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (country is null)
            {
                throw new NotFoundException(nameof(GetDetails), id);
            }

            return country;
        }
    }
}