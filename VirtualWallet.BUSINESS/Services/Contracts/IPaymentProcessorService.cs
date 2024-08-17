using ForumProject.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IPaymentProcessorService
    {
        public Task<string> VerifyAndRetrieveTokenAsync(Card card);
        public Task<bool> WithdrawFromRealCardAsync(string paymentProcessorToken, decimal amount);
        public Task<bool> DepositToRealCardAsync(string paymentProcessorToken, decimal amount);
    }

}
