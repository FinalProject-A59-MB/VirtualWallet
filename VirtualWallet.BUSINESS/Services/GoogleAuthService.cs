using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Helpers;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication;
using Twilio.Http;

namespace VirtualWallet.BUSINESS.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public GoogleAuthService(IUserService userService, IAuthService authService, IEmailService emailService)
        {
            _userService = userService;
            _authService = authService;
            _emailService = emailService;
        }

        public AuthenticationProperties GetGoogleLoginProperties(string redirectUri)
        {
            return new AuthenticationProperties { RedirectUri = redirectUri };
        }

        public async Task<Result<string>> ProcessGoogleLoginResponse(AuthenticateResult result)
        {
            if (result?.Principal == null)
            {
                return Result<string>.Failure("An error occurred while logging in with Google. Please try again.");
            }

            IEnumerable<Claim> claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            string email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (email == null)
            {
                return Result<string>.Failure("Unable to retrieve email from Google. Please try again.");
            }

            Result<User> existingUser = await _userService.GetUserByEmailAsync(email);

            if (existingUser.Value != null)
            {
                string token = _authService.GenerateToken(existingUser.Value);
                return Result<string>.Success(token);
            }

            return Result<string>.Failure("User does not exist. Please register.");
        }


        public async Task<Result<User>> ProcessGoogleRegisterResponseWithoutUrl(AuthenticateResult result)
        {
            if (result?.Principal == null)
            {
                return Result<User>.Failure("An error occurred while registering with Google. Please try again.");
            }

            IEnumerable<Claim> claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            string email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string firstName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            string lastName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

            if (email == null)
            {
                return Result<User>.Failure("Unable to retrieve email from Google. Please try again.");
            }

            Result<User> existingUser = await _userService.GetUserByEmailAsync(email);

            if (existingUser.Value == null)
            {
                User user = new User
                {
                    Email = email,
                    Username = email,
                    Password = PasswordGenerator.GenerateSecurePassword(),
                    UserProfile = new UserProfile
                    {
                        FirstName = firstName,
                        LastName = lastName,
                    },
                    Role = UserRole.RegisteredUser,
                    VerificationStatus = UserVerificationStatus.NotVerified
                };

                Result<User> registerResult = await _userService.RegisterUserAsync(user);

                if (!registerResult.IsSuccess)
                {
                    return Result<User>.Failure(registerResult.Error);
                }

                return Result<User>.Success(user);
            }

            return Result<User>.Failure("User already exists.");
        }

    }


}
