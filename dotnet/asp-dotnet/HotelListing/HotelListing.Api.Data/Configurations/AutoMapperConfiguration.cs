using AutoMapper;
using HotelListing.Api.Data.Entities;
using HotelListing.Api.Data.Models.Country;
using HotelListing.Api.Data.Models.Hotel;
using HotelListing.Api.Data.Models.Users;

namespace HotelListing.Api.Data.Configurations
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

            CreateMap<UserDto, UserEntity>()
                .ReverseMap();
        }
    }
}