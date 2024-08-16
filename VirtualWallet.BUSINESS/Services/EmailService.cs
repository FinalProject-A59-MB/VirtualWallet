using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace VirtualWallet.BUSINESS.Services
{
    public class EmailService
    {
        private readonly string _sendGridApiKey;

        public EmailService(string sendGridApiKey)
        {
            _sendGridApiKey = sendGridApiKey;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("your-email@example.com", "Your Name");
            var to = new EmailAddress(toEmail);
            var htmlContent = "<strong>additional content</strong>";
            var emailMessage = MailHelper.CreateSingleEmail(from, to, subject, message, htmlContent);
            await client.SendEmailAsync(emailMessage).ConfigureAwait(false);
        }
    }
}
