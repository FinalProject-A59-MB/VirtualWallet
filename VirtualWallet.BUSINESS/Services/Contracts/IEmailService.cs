using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(string toEmail, string subject, string message);


        Task<Result> SendVerificationEmailAsync(User user, string verificationLink);

        Task<Result> SendPasswordResetEmailAsync(User user, string resetLink);

    }

}