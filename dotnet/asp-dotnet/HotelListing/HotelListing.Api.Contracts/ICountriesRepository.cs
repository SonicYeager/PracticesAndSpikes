using HotelListing.Api.Data.Entities;
using HotelListing.Api.Data.Models.Country;

namespace HotelListing.Api.Contracts
{
    public interface ICountriesRepository : IRepository<CountryEntity>
    {
        public Task<CountryDto> GetDetails(int id);
    }
}