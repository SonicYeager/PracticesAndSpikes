using HotelListing.Api.Contracts;
using HotelListing.Api.Data.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IAuthManager authManager, ILogger<UsersController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        // POST: HotelListing/User/Register
        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] UserDto userDto)
        {
            try
            {
                _logger.LogInformation($"Registration Attempt for {userDto.Email}");

                var errors = await _authManager.Register(userDto);

                var identityErrorList = errors.ToList();

                if (identityErrorList.Any())
                {
                    foreach (var error in identityErrorList)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return BadRequest(ModelState);
                }

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something Went Wrong in the {nameof(Register)} - User Registration attempt for {userDto.Email}");
                return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        // POST: HotelListing/User/Login
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation($"Login Attempt for {loginDto.Email}");

                var authDto = await _authManager.Login(loginDto);

                if (authDto == null)
                {
                    return Unauthorized();
                }

                return Ok(authDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something Went Wrong in the {nameof(Login)} - User Login attempt for {loginDto.Email}");
                return Problem($"Something Went Wrong in the {nameof(Login)}", statusCode: 500);
            }
        }

        // POST: HotelListing/User/RefreshToken
        [HttpPost]
        [Route("RefreshToken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthDto>> RefreshToken([FromBody] AuthDto authDto)
        {
            var verifiedAuthDto = await _authManager.VerifyRefreshToken(authDto);

            if (verifiedAuthDto == null)
            {
                return Unauthorized();
            }

            return Ok(verifiedAuthDto);
        }
    }
}
