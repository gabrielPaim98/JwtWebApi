using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace JwtWebApi.Services.Mail
{
    public class MailService : IMailService
    {

        private readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendPasswordResetEmail(string email, string token)
        {
            SendEmail(email, "Reset Password Request", $"<a href=\"{token}\">Click here to reset your password</a>");
        }

        public void SendVerifyEmail(string email, string token)
        {
            SendEmail(email, $"Please Verify Your Email", $"<a href=\"{token}\">Click here to verify your email</a>");
        }

        private void SendEmail(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body }; // You can use a email template here.

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
