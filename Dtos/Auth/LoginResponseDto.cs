namespace JwtWebApi.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenCreatedOn { get; set; }
        public DateTime TokenExpiresIn { get; set; }

    }
}
