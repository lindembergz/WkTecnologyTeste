using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;
using UserAuth.Dominio.Servicos;

namespace UserAuth.Infraestrutura.Servicos
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailConfirmationAsync(string email, string token)
        {
            var subject = "Confirmação de E-mail";
            var confirmationUrl = $"{_configuration["EmailSettings:ConfirmationBaseUrl"]}/confirm-email?email={email}&token={token}";
            var body = @$"Por favor, confirme seu e-mail clicando neste link: <a href=""{confirmationUrl}"">Confirmar E-mail</a>";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendTwoFactorCodeAsync(string email, string code)
        {
            var subject = "Código de Autenticação de Dois Fatores";
            var body = @$"Seu código de autenticação de dois fatores é: <b>{code}</b>. Este código é válido por 5 minutos.";
            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = _configuration["EmailSettings:SmtpPort"];
            var smtpUser = _configuration["EmailSettings:Username"];
            var smtpPass = _configuration["EmailSettings:Password"];

            if (fromEmail == null || smtpServer == null || smtpPort == null || smtpUser == null || smtpPass == null)
            {
                throw new InvalidOperationException("Email settings are not configured.");
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(fromEmail));
            emailMessage.To.Add(MailboxAddress.Parse(toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                smtpServer,
                int.Parse(smtpPort),
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(
                smtpUser,
                smtpPass
            );
            await smtp.SendAsync(emailMessage);
            await smtp.DisconnectAsync(true);
        }
    }
}
