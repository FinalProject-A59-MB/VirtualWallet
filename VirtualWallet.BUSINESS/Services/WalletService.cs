using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.DATA.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public WalletService(IWalletRepository walletRepository,
            IUserService userService,
            IEmailService emailService)
        {
            _walletRepository = walletRepository;
            _userService = userService;
            _emailService = emailService;
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

        public async Task<Result<int>> GetWalletIdByUserDetailsAsync(string details)
        {
            var wallet = await _walletRepository.GetWalletByUserDetailsAsync(details);

            if (wallet == null)
            {
                return Result<int>.Failure(ErrorMessages.WalletNotFound);
            }

            return Result<int>.Success(wallet.Id);
        }

        public async Task<Result> AddUserToJointWalletAsync(int walletId, string username)
        {
            var walletResult = await GetWalletByIdAsync(walletId);

            if (!walletResult.IsSuccess)
            {
                return Result.Failure(ErrorMessages.WalletNotFound);
            }

            if(walletResult.Value.WalletType != Models.Enums.WalletType.Joint)
            {
                return Result.Failure(ErrorMessages.InvalidWalletInformation);
            }


            var userResult = await _userService.GetUserByUsernameAsync(username);

            if (!userResult.IsSuccess)
            {
                return Result.Failure(ErrorMessages.UserNotFound);
            }

            await _walletRepository.AddUserToJointWalletAsync(walletId, userResult.Value.Id);

            await _emailService.SendEmailAsync(userResult.Value.Email, "You were added to a wallet.", $"You were added to {walletResult.Value.Name}.");

            return Result.Success();
        }

        public async Task<Result> RemoveUserFromJointWalletAsync(int walletId, string username)
        {
            var walletResult = await GetWalletByIdAsync(walletId);

            if (!walletResult.IsSuccess)
            {
                return Result.Failure(ErrorMessages.WalletNotFound);
            }

            if (walletResult.Value.WalletType != Models.Enums.WalletType.Joint)
            {
                return Result.Failure(ErrorMessages.InvalidWalletInformation);
            }


            var userResult = await _userService.GetUserByUsernameAsync(username);

            if (!userResult.IsSuccess)
            {
                return Result.Failure(ErrorMessages.UserNotFound);
            }

            await _walletRepository.RemoveUserFromJointWalletAsync(walletId, userResult.Value.Id);

            await _emailService.SendEmailAsync(userResult.Value.Email, "You were removed from a wallet.", $"You were removed from {walletResult.Value.Name}.");

            return Result.Success();
        }

        public async Task<Result<IEnumerable<WalletTransaction>>> GetWalletTransactionsAsync(WalletTransactionQueryParameters filterParameters, int userId)
        {
            var transactions = await _walletRepository.FilterByAsync(filterParameters, userId);

            return transactions != null
                ? Result<IEnumerable<WalletTransaction>>.Success(transactions)
                : Result<IEnumerable<WalletTransaction>>.Failure("Transactions not found.");
        }

        public async Task<Result<int>> GetTotalWalletTransactionCountAsync(WalletTransactionQueryParameters filterParameters, int userId)
        {
            var totalCount = await _walletRepository.GetTotalCountAsync(filterParameters, userId);
            return Result<int>.Success(totalCount);
        }
    }
}
