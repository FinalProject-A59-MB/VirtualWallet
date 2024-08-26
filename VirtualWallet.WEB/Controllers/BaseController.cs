using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VirtualWallet.WEB.Controllers
{
    public abstract class BaseController : Controller
    {
        protected User CurrentUser => HttpContext.Items["CurrentUser"] as User;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
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
                
            }

            base.OnActionExecuting(context);
        }
    }
}
