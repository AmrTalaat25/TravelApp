using HandlebarsDotNet;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TravelApp.Dto;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Models.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string To, string Subject, string Body)
        {
            var emailSender = _config.GetSection("EmailSender");
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailSender["Username"]));
            email.To.Add(MailboxAddress.Parse(To));
            email.Subject = Subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = Body };

            using var smtp = new SmtpClient();
            smtp.Connect(emailSender["Host"], 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSender["Username"], emailSender["Password"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
