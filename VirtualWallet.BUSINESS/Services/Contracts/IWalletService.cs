using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Services.Contracts
{
    public interface IWalletService
    {
        Wallet GetWalletById(int id);
        IEnumerable<Wallet> GetWalletsByUserId(int userId);
        Wallet GetWalletByName(string walletName);
        void AddWallet(Wallet wallet);
        void UpdateWallet(Wallet wallet);
        void RemoveWallet(int walletId);
    }
}
