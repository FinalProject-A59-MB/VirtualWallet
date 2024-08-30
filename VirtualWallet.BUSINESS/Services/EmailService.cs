using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Result> SendEmailAsync(string toEmail, string subject, string message)
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

        return Result.Success();
    }

    public async Task<Result> SendVerificationEmailAsync(User user, string verificationLink)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "VirtualWallet.BUSINESS", "Resources", "EmailTemplates", "VerificationEmailTemplate.html");

        if (!File.Exists(templatePath))
        {
            return Result.Failure("Email template not found.");
        }

        var emailTemplate = await File.ReadAllTextAsync(templatePath);

        string emailContent = emailTemplate.Replace("{{Username}}", user.Username)
                                           .Replace("{{VerificationLink}}", verificationLink);

        await SendEmailAsync(user.Email, "Email Verification", emailContent);

        return Result.Success();

    }


}
