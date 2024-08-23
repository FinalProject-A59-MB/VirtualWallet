using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VirtualWallet.BUSINESS.Results;

public class HandleServiceResultAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ViewResult viewResult)
        {
            var controller = context.Controller as Controller;

            if (viewResult.Model is Result result)
            {
                if (!result.IsSuccess)
                {
                    controller.TempData["ErrorMessage"] = result.Error;

                }
                else
                {
                    controller.TempData["ErrorMessage"] = string.Empty;
                }
            }
            else if (viewResult.Model is Result<object> genericResult && !genericResult.IsSuccess)
            {
                controller.TempData["ErrorMessage"] = genericResult.Error;
            }
        }


        base.OnActionExecuted(context);
    }
}
