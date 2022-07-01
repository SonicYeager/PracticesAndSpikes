using AutoMapper;
using HotelListing.Contracts;
using HotelListing.Entities;
using HotelListing.Models.Configurations;
using HotelListing.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger<AuthManager> _logger;

        public AuthManager(IMapper mapper, UserManager<UserEntity> userManager, ILogger<AuthManager> logger)
        {
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IEnumerable<IdentityError>> Register(UserDto userDto)
        {
            var user = _mapper.Map<UserEntity>(userDto);
            user.UserName = userDto.Email;

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleConfiguration.USER_ROLE_NAME);
            }

            return result.Errors;
        }

        public async Task<bool> Login(LoginDto loginDto)
        {
            var isValidUser = false;

            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                isValidUser = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, "Could not validate user!");
            }

            return isValidUser;
        }
    }
}