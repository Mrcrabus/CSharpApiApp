using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Dto;
using WebApplication1.Model;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[Route("api/users")]

public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly PostsService _postsService;

    public UserController(UserManager<User> userManager, PostsService postsService)
    {
        _postsService = postsService;
        _userManager = userManager;
    }


    [HttpGet()]
    public async Task<IActionResult> GetUsersAll()
    {
        var users = await _userManager.Users
            .Include(user => user.Posts)
            .Select(user => new UserDto
            {
                Id = user.Id,
                UserName = user.DisplayName,
                Email = user.Email,
                Posts = user.Posts.Select(post => new Post
                {
                    Id = post.Id,
                    Title = post.Title,
                    Description = post.Description
                }).ToArray()
            })
            .ToListAsync();


        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsersById([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var posts = await _postsService.UserPosts(id);

        if (user == null)
        {
            return NotFound();
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.DisplayName,
            Email = user.Email,
            Posts = posts.ToArray()
        };

        return Ok(userDto);
    }
}