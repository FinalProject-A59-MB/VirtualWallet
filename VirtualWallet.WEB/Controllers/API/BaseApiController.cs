using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VirtualWallet.BUSINESS.Results;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.WEB.Controllers.API
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected User CurrentUser => HttpContext.Items["CurrentUser"] as User;

        protected IActionResult HandleAuthorization()
        {
            if (HttpContext.Items.ContainsKey("AuthorizationResult"))
            {
                var result = HttpContext.Items["AuthorizationResult"] as Result;

                if (result != null && !result.IsSuccess)
                {
                    return new ObjectResult(new { Error = result.Error })
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                }
            }

            return null; 
        }
    }
}
