using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace WebApplication1.Model;

public class User : IdentityUser
{
    public string DisplayName { get; set; }
    [JsonIgnore]
    public ICollection<Post> Posts { get; } = new List<Post>();
}