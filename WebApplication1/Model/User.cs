using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Model;

public class User: IdentityUser
{
    public string DisplayName { get; set; }
   
}