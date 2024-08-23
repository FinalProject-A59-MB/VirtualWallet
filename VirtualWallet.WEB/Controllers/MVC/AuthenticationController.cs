using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.ViewModels;

namespace ForumProject.Controllers.MVC
{
    public class AuthenticationController : Controller
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
                ModelState.AddModelError("CustomError", authResult.Error);
                return View(model);
            }

            var token = _authService.GenerateToken(authResult.Value);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });
            return RedirectToAction("Index", "Home");
        }

        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var claims = result.Principal.Identities
                                .FirstOrDefault()?.Claims
                                .Select(claim => new
                                {
                                    claim.Type,
                                    claim.Value
                                });
            }

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

            var userRequest = _viewModelMapper.ToUser(model);
            var registerResult = await _userService.RegisterUserAsync(userRequest);

            if (!registerResult.IsSuccess)
            {
                ModelState.AddModelError("CustomError", registerResult.Error);
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
                return View("VerificationResult", false);
            }

            var userId = _authService.GetUserIdFromToken(token);
            if (!userId.IsSuccess)
            {
                TempData["ErrorMessage"] = userId.Error;
                return View("VerificationResult", false);
            }

            var userResult = await _userService.GetUserByIdAsync(userId.Value);

            if (!userResult.IsSuccess)
            {
                TempData["ErrorMessage"] = userResult.Error;
                return View("VerificationResult", false);
            }

            var user = userResult.Value;
            user.VerificationStatus = UserVerificationStatus.Verified;
            var updateResult = await _userService.UpdateUserAsync(user);

            if (!updateResult.IsSuccess)
            {
                TempData["ErrorMessage"] = updateResult.Error;
                return View("VerificationResult", false);
            }

            var newToken = _authService.GenerateToken(user);
            HttpContext.Response.Cookies.Append("jwt", newToken, new CookieOptions { HttpOnly = true });

            return View("VerificationResult", true);
        }

        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
    }
}
