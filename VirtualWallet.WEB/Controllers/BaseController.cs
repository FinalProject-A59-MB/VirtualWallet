using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Results;

namespace VirtualWallet.WEB.Controllers
{
    public abstract class BaseController : Controller
    {
        protected User CurrentUser => HttpContext.Items["CurrentUser"] as User;


    }
}
