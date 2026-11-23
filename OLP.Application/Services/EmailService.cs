using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using OLP.Application.Contracts.Services;
using MailKit.Net.Smtp;

namespace OLP.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string email,
            string subject,
            string body)
        {
            var sender = Required("Email:SenderEmail");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("OLP", sender));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                Required("Email:Host"),
                int.Parse(Required("Email:Port")),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                Required("Email:Username"),
                Required("Email:Password"));

            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        private string Required(string key) =>
            _configuration[key]
            ?? throw new InvalidOperationException(
                $"Missing configuration: {key}");
    }
}