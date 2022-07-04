using HotelListing.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Contracts
{
    public interface IAuthManager
    {
        public Task<IEnumerable<IdentityError>> Register(UserDto userDto);
        public Task<AuthDto> Login(LoginDto loginDto);
        public Task<string> CreateRefreshToken();
        public Task<AuthDto> VerifyRefreshToken(AuthDto authDto);
    }
}