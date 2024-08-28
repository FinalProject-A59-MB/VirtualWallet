using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.DATA.Services
{
    public class UserWalletService : IUserWalletService
    {
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;

        public UserWalletService(IUserWalletRepository userWalletRepository, IWalletService walletService, IUserService userService)
        {
            _userWalletRepository = userWalletRepository;
            _userService = userService;
            _walletService = walletService;
        }


        public async Task<Result> AddUserWalletAsync(int walletId, int userId)
        {
            var wallet = await _walletService.GetWalletByIdAsync(walletId);

            if (wallet == null)
            {
                return Result<int>.Failure(ErrorMessages.InvalidWalletInformation);
            }

            if(_userService.GetUserByIdAsync(userId) == null)
            {
                return Result<int>.Failure(ErrorMessages.InvalidUserInformation);
            }

            if (wallet.Value.WalletType != WalletType.Joint)
            {
                return Result.Failure(ErrorMessages.InvalidUserInformation);
            }

            var userWallets = await GetUserWalletByWalletIdAsync(walletId);

            if (!userWallets.Value.Any(x => x.UserId == userId))
            {
                return Result.Failure(ErrorMessages.UserIsAlreadyAddedToWallet);
            }

            var userWalletId = await _userWalletRepository.AddUserWalletAsync(walletId, userId, UserWalletRole.Member);

            return userWalletId != 0
                ? Result<int>.Success(userWalletId)
                : Result<int>.Failure(ErrorMessages.InvalidWalletInformation);
        }

        public async Task<Result<IEnumerable<UserWallet>>> GetUserWalletsByUserIdAsync(int userId)
        {
            var wallet = await _userWalletRepository.GetUserWalletsByUserIdAsync(userId);

            return wallet != null
                ? Result<IEnumerable<UserWallet>>.Success(wallet)
                : Result<IEnumerable<UserWallet>>.Failure(ErrorMessages.InvalidWalletInformation);
        }

        public async Task<Result<IEnumerable<UserWallet>>> GetUserWalletByWalletIdAsync(int walletId)
        {
            var wallet = await _userWalletRepository.GetUserWalletByWalletIdAsync(walletId);

            return wallet != null
                ? Result<IEnumerable<UserWallet>>.Success(wallet)
                : Result<IEnumerable<UserWallet>>.Failure(ErrorMessages.InvalidWalletInformation);
        }

        public async Task<Result> RemoveUserWalletAsync(int walletId, int userId)
        {
            if (_walletService.GetWalletByIdAsync(walletId) == null)
            {
                return Result<int>.Failure(ErrorMessages.InvalidWalletInformation);
            }

            if (_userService.GetUserByIdAsync(userId) == null)
            {
                return Result<int>.Failure(ErrorMessages.InvalidUserInformation);
            }

            await _userWalletRepository.RemoveUserWalletAsync(walletId, userId);
            return Result.Success();
        }
    }
}
