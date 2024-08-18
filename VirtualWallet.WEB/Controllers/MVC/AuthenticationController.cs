using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Exceptions;
using VirtualWallet.BUSINESS.Services;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
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

        public AuthenticationController(IAuthService authService,IViewModelMapper modelMapper,IUserService userService)
        {
            _authService = authService;
            _viewModelMapper = modelMapper;
            _userService = userService;
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

            var returnUrl = HttpContext.Request.Cookies["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                HttpContext.Response.Cookies.Delete("ReturnUrl");
                return Redirect(returnUrl);
            }

            return RedirectToAction( "Index", "Home");
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

            try
            {
                User userRequest = _viewModelMapper.ToUser(model);



                var user = await _userService.RegisterUserAsync(userRequest);
                var token = _authService.GenerateToken(user);
                HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });

                return RedirectToAction("Index", "Home");
            }
            catch (DuplicateEntityException e)
            {
                ModelState.AddModelError("Username", e.Message);
                return View(model);
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = e.Message;
                Response.StatusCode = 500;
                return View("Error");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
    }
}
