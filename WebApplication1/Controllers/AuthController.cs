using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Dto;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;

        public AuthController(UserManager<User> userManager, TokenService tokenService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Create([FromBody] RegisterDto dto)
        {
            var user = new User() { UserName = dto.Email, DisplayName = dto.Name, Email = dto.Email };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            Response.Cookies.Append("accessToken", GetUserTokens(user).AccessToken);

            return Ok(GetUserTokens(user));

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return BadRequest("User is not exist");
            }

            return Ok(GetUserTokens(user!));

        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromBody] Tokens tokens)
        {

            var isValidate = _tokenService.TryRefreshToken(tokens, out var newTokens);

            return isValidate ? Ok(newTokens) : BadRequest("Tokens aren't validate");
        }


        private Tokens GetUserTokens(User user)
        {
            var claims = new[]
            {
                new Claim("id", user.Id),
                new Claim("email", user.Email!),
                new Claim("userName", user.DisplayName),

            };

            var tokens = _tokenService.GenerateTokens(claims);

            return tokens;
        }
    }
}