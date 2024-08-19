using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using VirtualWallet.BUSINESS.Services.Contracts;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var smtpSettings = _configuration.GetSection("EmailSettings");

        var smtpClient = new SmtpClient(smtpSettings["SmtpServer"])
        {
            Port = int.Parse(smtpSettings["SmtpPort"]),
            Credentials = new NetworkCredential(smtpSettings["SmtpUsername"], smtpSettings["SmtpPassword"]),
            EnableSsl = bool.Parse(smtpSettings["EnableSsl"]),
        };

        using (smtpClient)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["SmtpUsername"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
