using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{

    [Route("api/auth")]
    public class HomeController : ControllerBase
    {
        private static readonly Dictionary<string, User> Users = new()
        {
            {
                "admin@gmail.com", new User {Name = "admin", Email = "admin@gmail.com", Password = "lolkek"}
            }
        };

        [HttpGet("users")]
        public IEnumerable<User> GetAllUsers()
        {
            return Users.Values;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (Users.ContainsKey(dto.Email))
            {
                return BadRequest("User is exist already");
            }

            Users.Add(dto.Email, new User { Name = dto.Name, Email = dto.Email, Password = dto.Password });

            return Ok();
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto )
        {

            if (!Users.TryGetValue(dto.Email, out var user))
            {
                return BadRequest("User is not exist");
            }

            return Ok(user);
        }
    }
}
