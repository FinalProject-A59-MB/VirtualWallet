using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IAuthService
    {
        public Task<Result<User>> AuthenticateAsync(string identifier, string password);

        public Task<Result> ResetPasswordAsync(string email, string token, string newPassword);

        public string GenerateToken(User user);

        public Result<bool> ValidateToken(string token);

        public Result<int> GetUserIdFromToken(string token);

    }
}