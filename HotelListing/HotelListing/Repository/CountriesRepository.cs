using HotelListing.Contexts;
using HotelListing.Contracts;
using HotelListing.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Repository
{
    public class CountriesRepository : Repository<CountryEntity>, ICountriesRepository
    {
        public CountriesRepository(DbContext context) : base(context)
        {
        }
    }
}