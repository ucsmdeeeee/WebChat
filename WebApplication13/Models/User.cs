using System.ComponentModel.DataAnnotations;

namespace WebApplication13.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string ExternalId { get; set; } = string.Empty;
    }

}
