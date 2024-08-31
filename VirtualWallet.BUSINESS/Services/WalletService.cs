using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.DATA.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserService _userService;

        public WalletService(IWalletRepository walletRepository,
            IUserService userService)
        {
            _walletRepository = walletRepository;
            _userService = userService;
        }
        public async Task<Result<int>> AddWalletAsync(Wallet wallet)
        {
            if (wallet == null)
            {
                return Result<int>.Failure(ErrorMessages.InvalidWalletInformation);
            }

            var newWalletId = await _walletRepository.AddWalletAsync(wallet);

            return newWalletId != 0
                ? Result<int>.Success(newWalletId)
                : Result<int>.Failure(ErrorMessages.InvalidWalletInformation);
        }

        public async Task<Result<Wallet>> GetWalletByIdAsync(int id)
        {
            var wallet = await _walletRepository.GetWalletByIdAsync(id);

            return wallet != null
                ? Result<Wallet>.Success(wallet)
                : Result<Wallet>.Failure(ErrorMessages.InvalidWalletInformation);
        }

        public async Task<Result<Wallet>> GetWalletByNameAsync(string walletName)
        {
            var wallet = await _walletRepository.GetWalletByNameAsync(walletName);

            return wallet != null
                ? Result<Wallet>.Success(wallet)
                : Result<Wallet>.Failure(ErrorMessages.InvalidWalletInformation);
        }

        public async Task<Result<IEnumerable<Wallet>>> GetWalletsByUserIdAsync(int userId)
        {
            var wallets = await _walletRepository.GetWalletsByUserIdAsync(userId);

            return wallets != null
                ? Result<IEnumerable<Wallet>>.Success(wallets)
                : Result<IEnumerable<Wallet>>.Failure(ErrorMessages.InvalidWalletInformation);
        }

        public async Task<Result> RemoveWalletAsync(int walletId)
        {
            var walletResult = await GetWalletByIdAsync(walletId);

            if (!walletResult.IsSuccess)
            {
                return Result.Failure(ErrorMessages.WalletNotFound);
            }

            if (walletResult.Value.Balance > 0)
            {
                return Result.Failure(ErrorMessages.WalletNotEmpty);
            }

            await _walletRepository.RemoveWalletAsync(walletId);
            return Result.Success();
        }

        public async Task<Result> UpdateWalletAsync(Wallet wallet)
        {
            if (wallet == null)
            {
                return Result.Failure(ErrorMessages.InvalidWalletInformation);
            }

            var walletResult = await GetWalletByIdAsync(wallet.Id);

            if (!walletResult.IsSuccess)
            {
                return Result.Failure(ErrorMessages.WalletNotFound);
            }

            await _walletRepository.UpdateWalletAsync(wallet);
            return Result.Success();
        }

    }
}
