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
using VirtualWallet.WEB.Models.ViewModels.AuthenticationViewModels;
using VirtualWallet.WEB.Models.ViewModels;

namespace VirtualWallet.WEB.Controllers.API
{
    /// <summary>
    /// Controller responsible for handling authentication-related actions such as login, registration, and password management.
    /// </summary>
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationControllerApi : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IDtoMapper _dtoMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationControllerApi"/> class.
        /// </summary>
        /// <param name="authService">Service for handling authentication.</param>
        /// <param name="userService">Service for handling user-related actions.</param>
        /// <param name="emailService">Service for sending emails.</param>
        /// <param name="dtoMapper">Service for mapping DTOs to models.</param>
        public AuthenticationControllerApi(
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

        /// <summary>
        /// Logs in the user and generates a JWT token upon successful authentication.
        /// </summary>
        /// <param name="model">The login request containing username/email and password.</param>
        /// <returns>A JWT token if login is successful; otherwise, an error message.</returns>
        /// <response code="200">Returns the JWT token.</response>
        /// <response code="400">If the login credentials are invalid.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            Result<User> authResult = await _authService.AuthenticateAsync(model.UsernameOrEmail, model.Password);

            if (!authResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto {Error= authResult.Error });
            }

            string token = _authService.GenerateToken(authResult.Value);
            return Ok(new TokenResponseDto { Token = token });
        }


        /// <summary>
        /// Registers a new user and sends a verification email upon successful registration.
        /// </summary>
        /// <param name="model">The registration request containing user details.</param>
        /// <returns>A JWT token if registration is successful; otherwise, an error message.</returns>
        /// <response code="201">User created and returns a JWT token.</response>
        /// <response code="400">If the registration data is invalid.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            User userRequest = _dtoMapper.ToUser(model);
            Result<User> registerResult = await _userService.RegisterUserAsync(userRequest);

            if (!registerResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto { Error = registerResult.Error });
            }

            string token = _authService.GenerateToken(registerResult.Value);
            string verificationLink = Url.Action("VerifyEmail", "Authentication", new { token }, Request.Scheme);
            string emailContent = $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.";
            await _emailService.SendEmailAsync(registerResult.Value.Email, "Email Verification", emailContent);

            return StatusCode(StatusCodes.Status201Created, new TokenResponseDto { Token = token });
        }


        /// <summary>
        /// Verifies the user's email based on the provided token.
        /// </summary>
        /// <param name="token">The email verification token.</param>
        /// <returns>A new JWT token if verification is successful; otherwise, an error message.</returns>
        /// <response code="200">Email verified successfully and returns a new JWT token.</response>
        /// <response code="400">If the token is invalid or the user could not be found.</response>
        /// <response code="404">If the user associated with the token is not found.</response>
        /// <response code="500">If there was an error updating the user.</response>
        [HttpGet("verifyEmail")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var validateToken = _authService.ValidateToken(token);
            if (!validateToken.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto { Error = validateToken.Error });
            }

            Result<int> userId = _authService.GetUserIdFromToken(token);
            if (!userId.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto { Error = userId.Error });
            }

            Result<User> userResult = await _userService.GetUserByIdAsync(userId.Value);

            if (!userResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponseDto { Error = userResult.Error });
            }

            User user = userResult.Value;
            user.Role = UserRole.EmailVerifiedUser;
            Result updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto { Error = updateResult.Error });
            }

            string newToken = _authService.GenerateToken(user);
            return Ok(new TokenResponseDto { Token = newToken });
        }


        /// <summary>
        /// Initiates the password reset process by sending a reset link to the user's email.
        /// </summary>
        /// <param name="model">The request containing the user's email address.</param>
        /// <returns>A message indicating that the reset link has been sent.</returns>
        /// <response code="200">Reset link has been sent.</response>
        /// <response code="404">If the user with the provided email is not found.</response>
        [HttpPost("forgotPassword")]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {
            Result<User> userResult = await _userService.GetUserByEmailAsync(model.Email);

            if (!userResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponseDto { Error = userResult.Error });
            }

            string token = _authService.GenerateToken(userResult.Value);
            string resetLink = Url.Action("ResetPassword", "Authentication", new { token, email = model.Email }, Request.Scheme);
            string emailContent = $"You can reset your password by clicking <a href='{resetLink}'>here</a>.";

            await _emailService.SendEmailAsync(model.Email, "Password Reset", emailContent);

            return Ok(new MessageResponseDto { Message = "Password reset link has been sent to your email." });
        }


        /// <summary>
        /// Resets the user's password based on the provided token and new password.
        /// </summary>
        /// <param name="model">The reset password request containing the email, token, and new password.</param>
        /// <returns>A message indicating that the password has been reset successfully.</returns>
        /// <response code="200">Password reset successfully.</response>
        /// <response code="400">If the provided data is invalid.</response>
        [HttpPost("resetPassword")]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            Result resetResult = await _authService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);

            if (!resetResult.IsSuccess)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto { Error = resetResult.Error });
            }

            return Ok(new MessageResponseDto { Message = "Password has been reset successfully." });
        }


        /// <summary>
        /// Logs out the user by deleting the JWT cookie.
        /// </summary>
        /// <returns>A message indicating that the user has been logged out successfully.</returns>
        /// <response code="200">User logged out successfully.</response>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return Ok(new MessageResponseDto { Message = "Logged out successfully." });
        }


        /// <summary>
        /// Initiates the Google login process.
        /// </summary>
        /// <returns>A challenge result for Google authentication.</returns>
        /// <response code="302">Redirects to Google for authentication.</response>
        [HttpPost("googleLogin")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public IActionResult GoogleLogin()
        {
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleLoginResponse", "Authentication") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        /// <summary>
        /// Handles the response from Google after the user has authenticated.
        /// </summary>
        /// <returns>A JWT token if login is successful; otherwise, an error message.</returns>
        /// <response code="200">Returns the JWT token.</response>
        /// <response code="400">If an error occurred during Google login.</response>
        /// <response code="404">If the user associated with the email from Google login is not found.</response>
        [HttpGet("googleLoginResponse")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GoogleLoginResponse()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                IEnumerable<Claim> claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                string email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (email == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto { Error = "Unable to retrieve email from Google. Please try again." });
                }

                Result<User> existingUser = await _userService.GetUserByEmailAsync(email);

                if (existingUser.IsSuccess && existingUser.Value != null)
                {
                    string token = _authService.GenerateToken(existingUser.Value);
                    return Ok(new TokenResponseDto { Token = token });
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ErrorResponseDto { Error = existingUser.Error });
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto { Error = "An error occurred while logging in with Google. Please try again." });
        }


        /// <summary>
        /// Initiates the Google registration process.
        /// </summary>
        /// <returns>A challenge result for Google authentication.</returns>
        /// <response code="302">Redirects to Google for authentication.</response>
        [HttpPost("googleRegister")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public IActionResult GoogleRegister()
        {
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleRegisterResponse", "Authentication") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        /// <summary>
        /// Handles the response from Google after the user has registered.
        /// </summary>
        /// <returns>A JWT token if registration is successful; otherwise, an error message.</returns>
        /// <response code="200">Returns the JWT token.</response>
        /// <response code="400">If an error occurred during Google registration.</response>
        /// <response code="404">If the user associated with the email from Google registration is not found.</response>
        [HttpGet("googleRegisterResponse")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
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
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto { Error = "Unable to retrieve email from Google. Please try again." });
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
                        return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto { Error = registerResult.Error });
                    }

                    string token = _authService.GenerateToken(registerResult.Value);
                    string verificationLink = Url.Action("VerifyEmail", "Authentication", new { token }, Request.Scheme);
                    string emailContent = $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.";
                    await _emailService.SendEmailAsync(user.Email, "Email Verification", emailContent);

                    return Ok(new TokenResponseDto { Token = token });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto { Error = "User already exists. Please log in." });
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponseDto { Error = "An error occurred while registering with Google. Please try again." });
        }

    }
}
