namespace JwtWebApi.Services.Mail
{
    public interface IMailService
    {
        public void SendVerifyEmail(string email, string token);
        public void SendPasswordResetEmail(string email, string token);

    }
}
