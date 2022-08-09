using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Core.Repository
{
    public class HotelsRepository : Repository<HotelEntity>, IHotelsRepository
    {
        public HotelsRepository(DbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}