using System.ComponentModel.DataAnnotations;

namespace WebApplication13.Models
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; } = "User"; // Устанавливаем значение по умолчанию
    }

}
