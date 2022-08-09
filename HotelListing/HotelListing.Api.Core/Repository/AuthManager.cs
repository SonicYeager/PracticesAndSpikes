using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data.Configurations;
using HotelListing.Api.Data.Entities;
using HotelListing.Api.Data.Models.Configurations;
using HotelListing.Api.Data.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace HotelListing.Api.Core.Repository
{
    public class AuthManager : IAuthManager
    {
        public const string LOGIN_PROVIDER = "HotelListing";
        public const string REFRESH_TOKEN_NAME = "RefreshToken";

        private readonly IMapper _mapper;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthManager> _logger;
        private UserEntity _user;

        public AuthManager(IMapper mapper, UserManager<UserEntity> userManager, IConfiguration configuration, ILogger<AuthManager> logger)
        {
            _mapper = mapper;
            _userManager = userManager; ;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IEnumerable<IdentityError>> Register(UserDto userDto)
        {
            _user = _mapper.Map<UserEntity>(userDto);
            _user.UserName = userDto.Email;

            var result = await _userManager.CreateAsync(_user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, RoleConfiguration.USER_ROLE_NAME);
            }

            return result.Errors;
        }

        public async Task<AuthDto> Login(LoginDto loginDto)
        {
            _logger.LogInformation($"Looking for user with email {loginDto.Email}");
            _user = await _userManager.FindByEmailAsync(loginDto.Email);
            var isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);

            if (_user == null || isValidUser == false)
            {
                _logger.LogWarning($"User with email {loginDto.Email} was not found.");
                return null;
            }

            var token = await GenerateToken();

            return new AuthDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, LOGIN_PROVIDER, REFRESH_TOKEN_NAME);
            var newToken = await _userManager.GenerateUserTokenAsync(_user, LOGIN_PROVIDER, REFRESH_TOKEN_NAME);
            _ = await _userManager.SetAuthenticationTokenAsync(_user, LOGIN_PROVIDER, REFRESH_TOKEN_NAME, newToken);
            return newToken;
        }

        public async Task<AuthDto> VerifyRefreshToken(AuthDto authDto)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(authDto.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByNameAsync(username);

            if (_user == null)
            {
                return null;
            }

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, LOGIN_PROVIDER, REFRESH_TOKEN_NAME, authDto.Token);

            if (!isValidRefreshToken)
            {
                return null;
            }

            var token = await GenerateToken();
            await _userManager.UpdateSecurityStampAsync(_user);
            return new AuthDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };
        }

        private async Task<string> GenerateToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[AppSettingsKeys.JWT_SETTINGS_KEY]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var userClaims = await _userManager.GetClaimsAsync(_user);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, _user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, _user.Email),
            }
                .Union(userClaims)
                .Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration[AppSettingsKeys.JWT_SETTINGS_ISSUER],
                audience:_configuration[AppSettingsKeys.JWT_SETTINGS_AUDIENCE],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration[AppSettingsKeys.JWT_SETTINGS_DURATION])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}