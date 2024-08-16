using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IPaymentProcessorService
    {
        public Task<string> VerifyAndRetrieveTokenAsync(Card card);
        public Task<bool> ProcessDepositAsync(string paymentProcessorToken, int cardId, int walletId, decimal amount);
        public Task<bool> ProcessWithdrawalAsync(string paymentProcessorToken, int walletId, int cardId, decimal amount);
    }
}
