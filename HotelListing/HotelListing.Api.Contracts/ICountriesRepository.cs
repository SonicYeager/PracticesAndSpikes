using HotelListing.Api.Data.Entities;

namespace HotelListing.Api.Contracts
{
    public interface ICountriesRepository : IRepository<CountryEntity>
    {
        public Task<CountryEntity> GetDetails(int id);
    }
}