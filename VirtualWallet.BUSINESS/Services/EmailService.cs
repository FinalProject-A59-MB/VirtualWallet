using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using VirtualWallet.BUSINESS.Results;
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
        IConfigurationSection smtpSettings = _configuration.GetSection("EmailSettings");

        SmtpClient smtpClient = new SmtpClient(smtpSettings["SmtpServer"])
        {
            Port = int.Parse(smtpSettings["SmtpPort"]),
            Credentials = new NetworkCredential(smtpSettings["SmtpUsername"], smtpSettings["SmtpPassword"]),
            EnableSsl = bool.Parse(smtpSettings["EnableSsl"]),
        };

        using (smtpClient)
        {
            MailMessage mailMessage = new MailMessage
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
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "VirtualWallet.BUSINESS", "Resources", "EmailTemplates", "VerificationEmailTemplate.html");

        if (!File.Exists(templatePath))
        {
            return Result.Failure("Email template not found.");
        }

        string emailTemplate = await File.ReadAllTextAsync(templatePath);

        string emailContent = emailTemplate.Replace("{{Username}}", user.Username)
                                           .Replace("{{VerificationLink}}", verificationLink);

        await SendEmailAsync(user.Email, "Email Verification", emailContent);

        return Result.Success();

    }

    public async Task<Result> SendPasswordResetEmailAsync(User user, string resetLink)
    {
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "VirtualWallet.BUSINESS", "Resources", "EmailTemplates", "PasswordResetTemplate.html");

        if (!File.Exists(templatePath))
        {
            return Result.Failure("Email template not found.");
        }

        string emailTemplate = await File.ReadAllTextAsync(templatePath);

        string emailContent = emailTemplate.Replace("{{Username}}", user.Username)
                                           .Replace("{{ResetLink}}", resetLink);

        await SendEmailAsync(user.Email, "Password Reset", emailContent);

        return Result.Success();
    }

    public async Task<Result> SendPaymentVerificationEmailAsync(User user, string verificationCode)
    {
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "VirtualWallet.BUSINESS", "Resources", "EmailTemplates", "PaymentVerificationEmailTemplate.html");

        if (!File.Exists(templatePath))
        {
            return Result.Failure("Email template not found.");
        }

        string emailTemplate = await File.ReadAllTextAsync(templatePath);

        string emailContent = emailTemplate.Replace("{{Username}}", user.Username)
                                           .Replace("{{Generate}}", verificationCode);

        await SendEmailAsync(user.Email, "Payment Code", emailContent);

        return Result.Success();

    }


}
