using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Results;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ITransactionHandlingService
    {
        public Task<Result<CardTransaction>> ProcessCardToWalletTransactionAsync(Card card, Wallet wallet, decimal amount);
        
        public Task<Result<CardTransaction>> ProcessWalletToCardTransactionAsync(Wallet wallet, Card card, decimal amount, decimal feeAmout);

        Task<Result<WalletTransaction>> ProcessWalletToWalletTransactionAsync(WalletTransaction transaction);

    }
}