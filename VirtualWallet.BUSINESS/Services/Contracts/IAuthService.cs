using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IAuthService
    {
        public Task<User?> Authenticate(string credentials);

        public string GenerateToken(User user);

        public bool ValidateToken(string token);

        public int GetUserIdFromToken(string token);

    }
}