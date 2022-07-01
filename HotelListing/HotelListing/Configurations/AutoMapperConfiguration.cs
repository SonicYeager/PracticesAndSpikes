using AutoMapper;
using HotelListing.Entities;
using HotelListing.Models.Country;
using HotelListing.Models.Hotel;
using HotelListing.Models.Users;

namespace HotelListing.Configurations
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<CountryEntity, BaseCountryDto>().ReverseMap();
            CreateMap<CountryEntity, CreateCountryDto>().ReverseMap();
            CreateMap<CountryEntity, GetCountryDto>().ReverseMap();
            CreateMap<CountryEntity, CountryDto>().ReverseMap();
            CreateMap<CountryEntity, UpdateCountryDto>().ReverseMap();
            
            CreateMap<HotelEntity, BaseHotelDto>().ReverseMap();
            CreateMap<HotelEntity, CreateHotelDto>().ReverseMap();
            CreateMap<HotelEntity, GetHotelDto>().ReverseMap();
            CreateMap<HotelEntity, HotelDto>().ReverseMap();
            CreateMap<HotelEntity, UpdateCountryDto>().ReverseMap();

            CreateMap<UserDto, UserEntity>().ReverseMap();
        }
    }
}