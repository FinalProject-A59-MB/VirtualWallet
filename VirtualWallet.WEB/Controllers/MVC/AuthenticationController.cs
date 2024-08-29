using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.ViewModels;
using System.Security.Claims;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Helpers;
using VirtualWallet.WEB.Controllers;

namespace ForumProject.Controllers.MVC
{
    public class AuthenticationController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IViewModelMapper _viewModelMapper;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AuthenticationController(
            IAuthService authService,
            IViewModelMapper modelMapper,
            IUserService userService,
            IEmailService emailService)
        {
            _authService = authService;
            _viewModelMapper = modelMapper;
            _userService = userService;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var authResult = await _authService.AuthenticateAsync(model.UsernameOrEmail, model.Password);

            if (!authResult.IsSuccess)
            {
                TempData["ErrorMessage"] = authResult.Error;
                return View(model);
            }

            var token = _authService.GenerateToken(authResult.Value);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });
            return RedirectToAction("Index", "Home");
        }


        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleLoginResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleLoginResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (email == null)
                {
                    TempData["ErrorMessage"] = "Unable to retrieve email from Google. Please try again.";
                    return RedirectToAction("Login", "Authentication");
                }

                var existingUser = await _userService.GetUserByEmailAsync(email);

                if (existingUser.Value != null)
                {
                    var token = _authService.GenerateToken(existingUser.Value);
                    HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = existingUser.Error;
                    return RedirectToAction("Register", "Authentication");
                }
            }

            TempData["ErrorMessage"] = "An error occurred while logging in with Google. Please try again.";
            return RedirectToAction("Login", "Authentication");
        }

        public IActionResult GoogleRegister()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleRegisterResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleRegisterResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var firstName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
                var lastName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

                if (email == null)
                {
                    TempData["ErrorMessage"] = "Unable to retrieve email from Google. Please try again.";
                    return RedirectToAction("Register", "Authentication");
                }

                var existingUser = await _userService.GetUserByEmailAsync(email);

                if (existingUser.Value == null)
                {
                    var user = new User
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

                    var registerResult = await _userService.RegisterUserAsync(user);

                    if (!registerResult.IsSuccess)
                    {
                        TempData["ErrorMessage"] = registerResult.Error;
                        return RedirectToAction("Register", "Authentication");
                    }

                    var token = _authService.GenerateToken(registerResult.Value);
                    HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });

                    var verificationLink = Url.Action("VerifyEmail", "Authentication", new { token = token }, Request.Scheme);
                    string emailContent = $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.";
                    await _emailService.SendEmailAsync(user.Email, "Email Verification", emailContent);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = existingUser.Error;
                    return RedirectToAction("Login", "Authentication");
                }
            }

            TempData["ErrorMessage"] = "An error occurred while registering with Google. Please try again.";
            return RedirectToAction("Register", "Authentication");
        }




        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userRequest = _viewModelMapper.ToUser(model);
            var registerResult = await _userService.RegisterUserAsync(userRequest);

            if (!registerResult.IsSuccess)
            {
                TempData["ErrorMessage"] = registerResult.Error;
                return View(model);
            }

            var token = _authService.GenerateToken(registerResult.Value);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });

            var verificationLink = Url.Action("VerifyEmail", "Authentication", new { token = token }, Request.Scheme);
            string emailContent = $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.";
            await _emailService.SendEmailAsync(registerResult.Value.Email, "Email Verification", emailContent);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var validateToken = _authService.ValidateToken(token);
            if (!validateToken.IsSuccess)
            {
                TempData["ErrorMessage"] = validateToken.Error;
                return View("EmailVerification", false);
            }

            var userId = _authService.GetUserIdFromToken(token);
            if (!userId.IsSuccess)
            {
                TempData["ErrorMessage"] = userId.Error;
                return View("EmailVerification", false);
            }

            var userResult = await _userService.GetUserByIdAsync(userId.Value);

            if (!userResult.IsSuccess)
            {
                TempData["ErrorMessage"] = userResult.Error;
                return View("EmailVerification", false);
            }

            var user = userResult.Value;
            user.Role = UserRole.EmailVerifiedUser;
            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccess)
            {
                TempData["ErrorMessage"] = updateResult.Error;
                return View("EmailVerification", false);
            }

            var newToken = _authService.GenerateToken(user);
            HttpContext.Response.Cookies.Append("jwt", newToken, new CookieOptions { HttpOnly = true });

            return View("EmailVerification", true);
        }

        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userResult = await _userService.GetUserByEmailAsync(model.Email);

            if (userResult.IsSuccess)
            {
                var user = userResult.Value;
                var token = _authService.GenerateToken(user);

                var resetLink = Url.Action("ResetPassword", "Authentication", new { token, email = model.Email }, Request.Scheme);
                string emailContent = $"You can reset your password by clicking <a href='{resetLink}'>here</a>.";

                await _emailService.SendEmailAsync(model.Email, "Password Reset", emailContent);
                TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
            }
            else
            {
                TempData["ErrorMessage"] = userResult.Error;
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resetResult = await _authService.ResetPasswordAsync(model.Email, model.Token, model.Password);

            if (resetResult.IsSuccess)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully. You can now log in with your new password.";
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", resetResult.Error);
                return View();
            }
        }


    }
}
