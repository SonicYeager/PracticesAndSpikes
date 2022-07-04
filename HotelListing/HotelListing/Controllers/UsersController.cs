using HotelListing.Contracts;
using HotelListing.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public UsersController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        // POST: HotelListing/User/Register
        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] UserDto userDto)
        {
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

        // POST: HotelListing/User/Login
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthDto>> Login([FromBody] LoginDto loginDto)
        {
            var authDto = await _authManager.Login(loginDto);

            if (authDto == null)
            {
                return Unauthorized();
            }

            return Ok(authDto);
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
