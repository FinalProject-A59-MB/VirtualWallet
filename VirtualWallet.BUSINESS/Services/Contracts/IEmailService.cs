using SendGrid.Helpers.Mail;
using SendGrid;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string message);
    }
}