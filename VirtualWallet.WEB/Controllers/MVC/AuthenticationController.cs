using ForumProject.Models.DTOs;
using ForumProject.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using ForumProject.Models.ViewModels;
using ForumProject.Exceptions;
using System.Threading.Tasks;
using VirtualWallet.BUSINESS.Exceptions;
using VirtualWallet.BUSINESS.Services.Contracts;

namespace ForumProject.Controllers.MVC
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
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

            try
            {
                var user = await _authService.AuthenticateAsync($"{model.Username}:{model.Password}");
                var token = _authService.GenerateToken(user);
                HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true });

                var returnUrl = HttpContext.Request.Cookies["ReturnUrl"];
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    HttpContext.Response.Cookies.Delete("ReturnUrl");
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View(model);
            }
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
                var userRequest = new UserRequestDto
                {
                    Username = model.Username,
                    Password = model.Password,
                    Email = model.Email
                };

                var user = await _authService.RegisterAsync(userRequest);
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
