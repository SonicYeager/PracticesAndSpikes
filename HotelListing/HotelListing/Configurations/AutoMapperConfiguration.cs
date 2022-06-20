using AutoMapper;
using HotelListing.Entities;
using HotelListing.Models.Country;
using HotelListing.Models.Hotel;

namespace HotelListing.Configurations
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<CountryEntity, CreateCountryDto>().ReverseMap();
            CreateMap<CountryEntity, GetCountryDto>().ReverseMap();
            CreateMap<CountryEntity, CountryDto>().ReverseMap();
            CreateMap<CountryEntity, UpdateCountryDto>().ReverseMap();
            
            CreateMap<HotelEntity, HotelDto>().ReverseMap();
        }
    }
}