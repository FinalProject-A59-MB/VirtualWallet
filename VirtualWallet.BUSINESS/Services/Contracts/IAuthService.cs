using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IAuthService
    {
        public Task<Result<User>> AuthenticateAsync(string identifier, string password);

        public string GenerateToken(User user);

        public Result<bool> ValidateToken(string token);

        public Result<int> GetUserIdFromToken(string token);

    }
}