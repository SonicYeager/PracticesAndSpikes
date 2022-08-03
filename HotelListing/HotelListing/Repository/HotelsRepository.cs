using AutoMapper;
using HotelListing.Contracts;
using HotelListing.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Repository
{
    public class HotelsRepository : Repository<HotelEntity>, IHotelsRepository
    {
        public HotelsRepository(DbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}