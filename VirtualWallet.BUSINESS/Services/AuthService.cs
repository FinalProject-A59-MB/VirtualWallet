using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VirtualWallet.BUSINESS.Resources;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Helpers;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Repositories.Contracts;

namespace VirtualWallet.BUSINESS.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<Result<User>> AuthenticateAsync(string identifier, string password)
        {
            User? user = await _userRepository.GetUserByUsernameAsync(identifier) ??
                         await _userRepository.GetUserByEmailAsync(identifier);

            if (user == null || !PasswordHasher.VerifyPassword(password, user.Password))
                return Result<User>.Failure(ErrorMessages.InvalidCredentials);

            return Result<User>.Success(user);
        }

        public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
        {
            Result<bool> validateTokenResult = ValidateToken(token);
            if (!validateTokenResult.IsSuccess)
            {
                return Result.Failure(validateTokenResult.Error);
            }

            User user = await _userRepository.GetUserByEmailAsync(email);
            if (user==null)
            {
                return Result.Failure("Invalid email.");
            }

            user.Password = PasswordHasher.HashPassword(newPassword);

            await _userRepository.UpdateUserAsync(user);

            return Result.Success();
        }

        public string GenerateToken(User user)
        {
            IConfigurationSection jwtSettings = _configuration.GetSection("Jwt");
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Result<bool> ValidateToken(string token)
        {
            IConfigurationSection jwtSettings = _configuration.GetSection("Jwt");
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Token validation failed: {ex.Message}");
            }
        }

        public Result<int> GetUserIdFromToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            IConfigurationSection jwtSettings = _configuration.GetSection("Jwt");
            byte[] key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            try
            {
                TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                JwtSecurityToken jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null)
                {
                    return Result<int>.Failure("Invalid token format");
                }

                Claim userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Result<int>.Failure("Token does not contain user ID claim");
                }

                return Result<int>.Success(int.Parse(userIdClaim.Value));
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Failed to extract user ID from token: {ex.Message}");
            }
        }
    }
}
