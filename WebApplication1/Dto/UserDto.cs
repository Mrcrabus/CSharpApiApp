using System.ComponentModel.DataAnnotations;
using WebApplication1.Model;

namespace WebApplication1.Dto;

public class UserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    [EmailAddress()]
    public string Email { get; set; }
    public Post[] Posts { get; set; }
}