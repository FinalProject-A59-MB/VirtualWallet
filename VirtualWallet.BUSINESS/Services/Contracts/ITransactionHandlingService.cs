using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Results;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ITransactionHandlingService
    {
        public Task<Result<CardTransaction>> ProcessCardToWalletTransactionAsync(Card card, Wallet wallet, decimal amount);
        
        public Task<Result<CardTransaction>> ProcessWalletToCardTransactionAsync(Wallet wallet, Card card, decimal amount, decimal feeAmout);

        public Task<Result<int>> ProcessWalletToWalletTransactionStep1Async(Wallet senderWallet, Wallet recipientWallet, decimal amount);

        public Task<Result<int>> ProcessWalletToWalletTransactionStep2Async(Wallet senderWallet, Wallet recipientWallet, WalletTransaction walletTransaction);
    }
}