using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.WEB.Controllers
{
    public class ErrorController : BaseController
    {
        
        public IActionResult Index(int? statusCode = null, string message = null)
        {
            if (statusCode.HasValue)
            {
                ViewBag.StatusCode = statusCode.Value;
            }

            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            return View();
        }
    }
}
