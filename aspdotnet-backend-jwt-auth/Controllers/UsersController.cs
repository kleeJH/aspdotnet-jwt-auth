using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JWTBackendAuth.Models;
using JWTBackendAuth.Repository;
using JWTBackendAuth.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace JWTBackendAuth.Controllers
{
    [Authorize]
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IJWTManagerRepository _jWTManager;
        private readonly IUserServiceRepository _userServiceRepository;


        public UsersController(IJWTManagerRepository jWTManager, IUserServiceRepository userServiceRepository)
        {
            this._jWTManager = jWTManager;
            this._userServiceRepository = userServiceRepository;
        }

        [HttpGet]
        public IActionResult GetRandomData()
        {
            var data = new List<string>
            {
            "I love my apple pie!",
            "Please tell me something that I did not know...",
            "An apple a day, keeps the doctor away!"
            };

            String output = ResponsePackager.GetResponse("Message retrieved perfectly", data);

            return Ok(output);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserModel model)
        {
            UserCreatedModel result = await _userServiceRepository.CreateUserAsync(model);

            if (result.Succeeded)
            {

                return Ok("Successfully created user!");
            }
            else
            {
                // result.Message might return one or more errors
                return BadRequest(ResponsePackager.PostResponse(result.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] LoginModel login)
        {
            var validUser = await _userServiceRepository.IsValidUserAsync(login);
            var user = await _userServiceRepository.GetIdentityUser(login.Id);

            if (!validUser)
            {
                return Unauthorized("Incorrect username or password!");
            }

            var token = _jWTManager.GenerateToken(user.UserName!);

            if (!token.HasValue)
            {
                return Unauthorized("Invalid Attempt!");
            }

            // Save refresh token in DB
            RefreshTokenModel obj = new()
            {
                RefreshToken = token.Value.RefreshToken,
                UserName = user.UserName!,
            };

            _userServiceRepository.AddUserRefreshTokens(obj);
            _userServiceRepository.SaveCommit();
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenModel token)
        {
            try
            {
                var principal = _jWTManager.GetPrincipalFromExpiredToken(token.AccessToken);
                var username = principal.Identity?.Name;

                // Retrieve the saved refresh token from DB
                RefreshTokenModel? savedRefreshToken = _userServiceRepository.GetSavedRefreshTokens(username!, token.RefreshToken);

                if (savedRefreshToken!.RefreshToken != token.RefreshToken)
                {
                    return Unauthorized("Invalid attempt!");
                }

                var newJwtToken = _jWTManager.GenerateRefreshToken(username!);

                if (!newJwtToken.HasValue)
                {
                    return Unauthorized("Invalid attempt!");
                }

                // Save new refresh token in DB
                RefreshTokenModel obj = new()
                {
                    RefreshToken = newJwtToken.Value.RefreshToken,
                    UserName = username!
                };

                _userServiceRepository.DeleteUserRefreshTokens(username!, token.RefreshToken);
                _userServiceRepository.AddUserRefreshTokens(obj);
                _userServiceRepository.SaveCommit();

                return Ok(newJwtToken);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest("Something unexcpected has happened.");
            }
        }
    }
}
