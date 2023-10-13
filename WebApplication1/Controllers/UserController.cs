using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Dto;
using WebApplication1.Model;
using Microsoft.AspNetCore.Cors;


namespace WebApplication1.Controllers;

[Route("api/users")]

public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UserController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [Authorize()]
    [HttpGet()]
    public async Task<IActionResult> GetUsersAll()
    {
        var users = await _userManager.Users
            .Select(user => new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
            })
            .ToListAsync();


        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsersById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
        

        return Ok(userDto);
    }
}