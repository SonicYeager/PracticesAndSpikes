using HotelListing.Api.Data.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Api.Contracts
{
    public interface IAuthManager
    {
        public Task<IEnumerable<IdentityError>> Register(UserDto userDto);
        public Task<AuthDto> Login(LoginDto loginDto);
        public Task<string> CreateRefreshToken();
        public Task<AuthDto> VerifyRefreshToken(AuthDto authDto);
    }
}