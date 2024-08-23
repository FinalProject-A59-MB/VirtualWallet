using VirtualWallet.BUSINESS.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Results;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IPaymentProcessorService
    {
        public Task<Result<string>> VerifyAndRetrieveTokenAsync(Card card);
        public Task<Result> WithdrawFromRealCardAsync(string paymentProcessorToken, decimal amount);
        public Task<Result> DepositToRealCardAsync(string paymentProcessorToken, decimal amount);
    }

}
