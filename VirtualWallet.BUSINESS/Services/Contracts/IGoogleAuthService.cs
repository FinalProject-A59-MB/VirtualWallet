using Microsoft.AspNetCore.Authentication;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.BUSINESS.Services.Contracts
{
    public interface IGoogleAuthService
    {

        Task<Result<string>> ProcessGoogleLoginResponse(AuthenticateResult result);

        Task<Result<User>> ProcessGoogleRegisterResponseWithoutUrl(AuthenticateResult result);

    }
}