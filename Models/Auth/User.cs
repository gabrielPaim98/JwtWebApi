namespace JwtWebApi.Models.Auth
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; } = false;
        public string Role { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? EmailVerificationToken { get; set; } = string.Empty;
        public string? PasswordResetToken { get; set; } = string.Empty;
        public DateTime? PasswordResetTokenExpiration { get; set; }
    }
}
