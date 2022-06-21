using HotelListing.Entities;

namespace HotelListing.Contracts
{
    public interface ICountriesRepository : IRepository<CountryEntity>
    {
        public Task<CountryEntity> GetDetails(int id);
    }
}