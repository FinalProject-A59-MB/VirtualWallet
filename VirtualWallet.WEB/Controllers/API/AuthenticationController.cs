using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Helpers;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.DTOs.AuthDTOs;
using VirtualWallet.BUSINESS.Results;

namespace VirtualWallet.WEB.Controllers.API
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IDtoMapper _dtoMapper;

        public AuthenticationController(
            IAuthService authService,
            IUserService userService,
            IEmailService emailService,
            IDtoMapper dtoMapper)
        {
            _authService = authService;
            _userService = userService;
            _emailService = emailService;
            _dtoMapper = dtoMapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {

            Result<User> authResult = await _authService.AuthenticateAsync(model.UsernameOrEmail, model.Password);

            if (!authResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, authResult.Error);
            }

            string token = _authService.GenerateToken(authResult.Value);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {

            User userRequest = _dtoMapper.ToUser(model);
            Result<User> registerResult = await _userService.RegisterUserAsync(userRequest);

            if (!registerResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, registerResult.Error);
            }

            string token = _authService.GenerateToken(registerResult.Value);
            string verificationLink = Url.Action("VerifyEmail", "AuthenticationApi", new { token }, Request.Scheme);
            string emailContent = $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.";
            await _emailService.SendEmailAsync(registerResult.Value.Email, "Email Verification", emailContent);

            return StatusCode(StatusCodes.Status201Created, new { Token = token });
        }

        [HttpGet("verifyEmail")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var validateToken = _authService.ValidateToken(token);
            if (!validateToken.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, validateToken.Error);
            }

            Result<int> userId = _authService.GetUserIdFromToken(token);
            if (!userId.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, userId.Error);
            }

            Result<User> userResult = await _userService.GetUserByIdAsync(userId.Value);

            if (!userResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, userResult.Error);
            }

            User user = userResult.Value;
            user.Role = UserRole.EmailVerifiedUser;
            Result updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, updateResult.Error);
            }

            string newToken = _authService.GenerateToken(user);
            return Ok(new { Token = newToken });
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {

            Result<User> userResult = await _userService.GetUserByEmailAsync(model.Email);

            if (!userResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, userResult.Error);
            }

            string token = _authService.GenerateToken(userResult.Value);
            string resetLink = Url.Action("ResetPassword", "AuthenticationApi", new { token, email = model.Email }, Request.Scheme);
            string emailContent = $"You can reset your password by clicking <a href='{resetLink}'>here</a>.";

            await _emailService.SendEmailAsync(model.Email, "Password Reset", emailContent);

            return Ok(new { Message = "Password reset link has been sent to your email." });
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            Result resetResult = await _authService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);

            if (!resetResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, resetResult.Error);
            }

            return Ok(new { Message = "Password has been reset successfully." });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return Ok(new { Message = "Logged out successfully." });
        }

        [HttpPost("googleLogin")]
        public IActionResult GoogleLogin()
        {
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleLoginResponse", "AuthenticationApi") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("googleLoginResponse")]
        public async Task<IActionResult> GoogleLoginResponse()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                IEnumerable<Claim> claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                string email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (email == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Unable to retrieve email from Google. Please try again.");
                }

                Result<User> existingUser = await _userService.GetUserByEmailAsync(email);

                if (existingUser.IsSuccess && existingUser.Value != null)
                {
                    string token = _authService.GenerateToken(existingUser.Value);
                    return Ok(new { Token = token });
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, existingUser.Error);
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, "An error occurred while logging in with Google. Please try again.");
        }

        [HttpPost("googleRegister")]
        public IActionResult GoogleRegister()
        {
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleRegisterResponse", "AuthenticationApi") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("googleRegisterResponse")]
        public async Task<IActionResult> GoogleRegisterResponse()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                IEnumerable<Claim> claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                string email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                string firstName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
                string lastName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

                if (email == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Unable to retrieve email from Google. Please try again.");
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
                        return StatusCode(StatusCodes.Status400BadRequest, registerResult.Error);
                    }

                    string token = _authService.GenerateToken(registerResult.Value);
                    string verificationLink = Url.Action("VerifyEmail", "AuthenticationApi", new { token }, Request.Scheme);
                    string emailContent = $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.";
                    await _emailService.SendEmailAsync(user.Email, "Email Verification", emailContent);

                    return Ok(new { Token = token });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "User already exists. Please log in.");
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, "An error occurred while registering with Google. Please try again.");
        }
    }
}
