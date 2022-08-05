using System.ComponentModel.DataAnnotations;

namespace JwtWebApi.Dtos.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public bool isAdmin { get; set; } = false;
    }
}
