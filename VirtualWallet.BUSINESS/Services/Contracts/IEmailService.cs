using SendGrid.Helpers.Mail;
using SendGrid;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }

}