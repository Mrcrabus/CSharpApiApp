using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[Route("api/users")]
public class UserController: ControllerBase
{
    
    private readonly UserService _userService;
    public UserController( UserService userService)
    {
        _userService = userService;
    }
    
    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> GetUsersAll()
    {
        var users = await _userService.GetUsers();
        return Ok(users);
    }   
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsersById(int id)
    {
        var user = await _userService.GetUserById(id);
        return Ok(user);
    }
    
    
}