using HotelListing.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Contracts
{
    public interface IAuthManager
    {
        public Task<IEnumerable<IdentityError>> Register(UserDto userDto);
        public Task<bool> Login(LoginDto loginDto);
    }
}