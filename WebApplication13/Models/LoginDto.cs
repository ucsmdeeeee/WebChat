using System.ComponentModel.DataAnnotations;
namespace WebApplication13.Models
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }
    }

}
