using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dto
{
    public class RegisterDto
    {
        [MinLength(5)]
        [MaxLength(30)]
        [Required]
        public string Name { get; set; }

        [EmailAddress]
        [MinLength(5)]
        [Required]
        public string Email { get; set; }


        [MaxLength(30)]
        [Required]
        public string Password { get; set; }         
    }
}
