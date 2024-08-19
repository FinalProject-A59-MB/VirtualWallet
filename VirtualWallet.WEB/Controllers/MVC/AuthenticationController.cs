using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Exceptions;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.WEB.Models.DTOs;
using VirtualWallet.WEB.Models.ViewModels;

namespace ForumProject.Controllers.MVC
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IViewModelMapper _viewModelMapper;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AuthenticationController(IAuthService authService,IViewModelMapper modelMapper,IUserService userService,IEmailService emailService)
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

            User user = await _authService.Authenticate($"{model.UsernameOrEmail}:{model.Password}");

            if (user == null)
            {
                ModelState.AddModelError("CustomError", "Invalid credentials. Please try again.");
                return View(model);
            }

            var token = _authService.GenerateToken(user);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });
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
            var user = await _userService.RegisterUserAsync(userRequest);
            var token = _authService.GenerateToken(user);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });
            var verificationLink = Url.Action("VerifyEmail", "Authentication", new { token = token }, Request.Scheme);
            string emailContent = $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.";
            await _emailService.SendEmailAsync(user.Email, "Email Verification", emailContent);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token) || !_authService.ValidateToken(token))
            {
                return View("VerificationResult", false);
            }

            var userId = _authService.GetUserIdFromToken(token);
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return View("VerificationResult", false);
            }
            user.VerificationStatus = UserVerificationStatus.Verified;
            await _userService.UpdateUserAsync(user);

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
