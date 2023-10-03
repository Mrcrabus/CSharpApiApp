using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;
using WebApplication1.Model;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public AuthController(UserService userService, TokenService tokenService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Create([FromBody] RegisterDto dto)
        {
            // var hashedPassword = HashPassword(dto.Password);
            var user = new User { Name = dto.Name, Email = dto.Email, Password = dto.Password };

            User? newUser = await _userService.CreateUser(user);

            if (newUser == null) return BadRequest("User already exist");

            return Ok(newUser);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            User? user = await _userService.GetUserByEmail(dto.Email);
            if (user == null)
            {
                return BadRequest("User is not exist");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
            };
            
            return Ok(_tokenService.GenerateTokens(claims));
        }


        // private static string HashPassword(string password)
        // {
        //     byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
        //     
        //     string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //         password: password!,
        //         salt: salt,
        //         prf: KeyDerivationPrf.HMACSHA256,
        //         iterationCount: 100000,
        //         numBytesRequested: 256 / 8));
        //
        //     return hashed;
        // }
        //
        // private static bool VerifyHashedPassword(string hashedPassword, string password)
        // {
        //     byte[] buffer4;
        //     byte[] src = Convert.FromBase64String(hashedPassword);
        //     if ((src.Length != 0x31) || (src[0] != 0))
        //     {
        //         return false;
        //     }
        //     byte[] dst = new byte[0x10];
        //     Buffer.BlockCopy(src, 1, dst, 0, 0x10);
        //     byte[] buffer3 = new byte[0x20];
        //     Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
        //     using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
        //     {
        //         buffer4 = bytes.GetBytes(0x20);
        //     }
        //     return ByteArraysEqual(buffer3, buffer4);
        // }
    }
}