using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, TokenService tokenService,
            SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
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
            // return Ok(GetUserTokens(user));
            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
            return Ok(userDto);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, true, false);
            if (!result.Succeeded)
            {
                return BadRequest("User is not exist");
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);

            Response.Cookies.Append("accessToken", GetUserTokens(user!).AccessToken);

            return Ok(GetUserTokens(user!));

            // var userDto = new UserDto
            // {
            //     Id = user.Id,
            //     UserName = user.UserName,
            //     Email = user.Email
            // };
            //
            // return Ok(userDto);
        }

        [HttpPost("sign-out")]
        public async void SignOut()
        {
            Response.Cookies.Delete("accessToken");
            await _signInManager.SignOutAsync();
        }

        // [HttpPost("refresh")]
        // public IActionResult RefreshToken()
        // {
        //     Response.Cookies.Append("accessToken", GetUserTokens(user!).AccessToken);
        //
        //     var accessToken = Request.Cookies[".AspNetCore.Identity.Application"];
        //
        //     var isValidate = _tokenService.TryRefreshToken(tokens, out var newTokens);
        //
        //
        //     return isValidate ? Ok(newTokens) : BadRequest("Tokens aren't validate");
        // }
        [Authorize()]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var accessToken = Request.Cookies["accessToken"];
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(accessToken);
            
            // var result = await _signInManager.RefreshSignInAsync();
            
            return Ok(jwtSecurityToken);
        }

        private Tokens GetUserTokens(User user)
        {
            var claims = new[]
            {
                new Claim("Email", user.Email),
                new Claim("Name", user.DisplayName),
            };

            var tokens = _tokenService.GenerateTokens(claims);

            return tokens;
        }
    }
}