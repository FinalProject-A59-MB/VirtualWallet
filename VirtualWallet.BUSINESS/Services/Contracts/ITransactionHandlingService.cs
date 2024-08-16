using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface ITransactionHandlingService
    {
        public Task<CardTransaction> ProcessCardToWalletTransactionAsync(Card card, Wallet wallet, decimal amount);

        public Task<CardTransaction> ProcessWalletToCardTransactionAsync(Wallet wallet, Card card, decimal amount);

        public Task<WalletTransaction> ProcessWalletToWalletTransactionAsync(Wallet senderWallet, Wallet recipientWallet, decimal amount);
    }
}