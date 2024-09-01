using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.WEB.Controllers.MVC;
using VirtualWallet.WEB.Models.ViewModels.AuthenticationViewModels;
using VirtualWallet.BUSINESS.Results;

namespace ForumProject.Controllers.MVC
{
    public class AuthenticationController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IViewModelMapper _viewModelMapper;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthenticationController(
            IAuthService authService,
            IViewModelMapper modelMapper,
            IUserService userService,
            IEmailService emailService,
            IGoogleAuthService googleAuthService)
        {
            _authService = authService;
            _viewModelMapper = modelMapper;
            _userService = userService;
            _emailService = emailService;
            _googleAuthService = googleAuthService;
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


            string token = _authService.GenerateToken(authResult.Value);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });
            return RedirectToAction("Index", "Home");
        }


        public IActionResult GoogleLogin()
        {
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleLoginResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleLoginResponse()
        {
            AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Result<string> result = await _googleAuthService.ProcessGoogleLoginResponse(authenticateResult);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction("Login", "Authentication");
            }

            HttpContext.Response.Cookies.Append("jwt", result.Value, new CookieOptions { HttpOnly = true });

            return RedirectToAction("Index", "Home");
        }


        public IActionResult GoogleRegister()
        {
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleRegisterResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleRegisterResponse()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Result<User> registerResult = await _googleAuthService.ProcessGoogleRegisterResponseWithoutUrl(result);

            if (!registerResult.IsSuccess)
            {
                TempData["ErrorMessage"] = registerResult.Error;
                return RedirectToAction("Register", "Authentication");
            }

            string token = _authService.GenerateToken(registerResult.Value);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });

            string verificationLink = Url.Action("VerifyEmail", "Authentication", new { token = token }, Request.Scheme);
            await _emailService.SendVerificationEmailAsync(registerResult.Value, verificationLink);

            return RedirectToAction("Index", "Home");
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

            User userRequest = _viewModelMapper.ToUser(model);
            Result<User> registerResult = await _userService.RegisterUserAsync(userRequest);

            if (!registerResult.IsSuccess)
            {
                TempData["ErrorMessage"] = registerResult.Error;
                return View(model);
            }


            string token = _authService.GenerateToken(registerResult.Value);
            string verificationLink = Url.Action("VerifyEmail", "Authentication", new { token = token }, Request.Scheme);

            var result = await _emailService.SendVerificationEmailAsync(registerResult.Value, verificationLink);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "A verification link has been sent to your email. Please use it to verify your account to gain full access.";

            }

            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ResendVerificationEmail(int id)
        {
            Result<User> userResult = await _userService.GetUserByIdAsync(id);
            if (!userResult.IsSuccess)
            {
                TempData["ErrorMessage"] = userResult.Error;
                return RedirectToAction("Index", "Home");
            }
            string token = _authService.GenerateToken(userResult.Value);
            var verificationLink = Url.Action("VerifyEmail", "Authentication", new { token = token }, Request.Scheme);

            var result = await _emailService.SendVerificationEmailAsync(userResult.Value, verificationLink);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "A verification link has been re-sent to your email. Please use it to verify your account to gain full access.";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            Result<bool> validateToken = _authService.ValidateToken(token);
            if (!validateToken.IsSuccess)
            {
                TempData["ErrorMessage"] = validateToken.Error;
                return View("EmailVerification", false);
            }

            Result<int> userId = _authService.GetUserIdFromToken(token);
            if (!userId.IsSuccess)
            {
                TempData["ErrorMessage"] = userId.Error;
                return View("EmailVerification", false);
            }

            Result<User> userResult = await _userService.GetUserByIdAsync(userId.Value);

            if (!userResult.IsSuccess)
            {
                TempData["ErrorMessage"] = userResult.Error;
                return View("EmailVerification", false);
            }

            User user = userResult.Value;
            user.Role = UserRole.EmailVerifiedUser;
            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccess)
            {
                TempData["ErrorMessage"] = updateResult.Error;
                return View("EmailVerification", false);
            }

            string newToken = _authService.GenerateToken(user);
            HttpContext.Response.Cookies.Append("jwt", newToken, new CookieOptions { HttpOnly = true });

            return View("EmailVerification", true);
        }

        public IActionResult EmailVerification()
        {
            return View();
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
                User user = userResult.Value;
                string token = _authService.GenerateToken(user);

                string resetLink = Url.Action("ResetPassword", "Authentication", new { token, email = model.Email }, Request.Scheme);
                var emailResult = await _emailService.SendPasswordResetEmailAsync(user, resetLink);

                if (emailResult.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
                }
                else
                {
                    TempData["ErrorMessage"] = emailResult.Error;
                }
            }
            else
            {
                TempData["ErrorMessage"] = userResult.Error;
                return View();
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel { Token = token, Email = email };
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
                return RedirectToAction("Index", "Home");
            }
        }


    }
}
