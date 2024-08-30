using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using VirtualWallet.BUSINESS.Results;

namespace VirtualWallet.WEB.Controllers.MVC
{
    public abstract class BaseController : Controller
    {
        protected User CurrentUser => HttpContext.Items["CurrentUser"] as User;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Items.ContainsKey("AuthorizationResult"))
            {
                var result = HttpContext.Items["AuthorizationResult"] as Result;

                if (result != null && !result.IsSuccess)
                {
                    TempData["ErrorMessage"] = result.Error;

                    context.Result = RedirectToAction("Index", "Home");
                }
            }

            if (CurrentUser != null)
            {
                ViewBag.UserId = CurrentUser.Id;
                ViewBag.Username = CurrentUser.Username;
                ViewBag.UserRole = CurrentUser.Role.ToString();
                ViewBag.IsAuthenticated = true;
                ViewBag.Wallets = CurrentUser.Wallets;
                ViewBag.Cards = CurrentUser.Cards;
            }
            else
            {
                ViewBag.UserId = null;
                ViewBag.Username = "Guest";
                ViewBag.UserRole = "Anonymous";
                ViewBag.IsAuthenticated = false;
                ViewBag.Wallets = null;
            }

            base.OnActionExecuting(context);
        }
    }
}
