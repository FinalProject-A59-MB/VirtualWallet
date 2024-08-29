using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Services.Contracts;
using VirtualWallet.BUSINESS.Results;

public class RequireAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly int _minRequiredRoleLevel;
    private IAuthService _authService;
    private IUserService _userService;

    public RequireAuthorizationAttribute(int minRequiredRoleLevel = 0)
    {
        _minRequiredRoleLevel = minRequiredRoleLevel;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        _authService = context.HttpContext.RequestServices.GetService<IAuthService>();
        _userService = context.HttpContext.RequestServices.GetService<IUserService>();

        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token == null)
        {
            token = context.HttpContext.Request.Cookies["jwt"];
        }

        if (string.IsNullOrEmpty(token))
        {
            HandleUnauthorizedRequest(context);
            return;
        }

        var validateTokenResult = _authService.ValidateToken(token);
        if (!validateTokenResult.IsSuccess)
        {
            HandleUnauthorizedRequest(context);
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
        {
            HandleUnauthorizedRequest(context);
            return;
        }

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            HandleUnauthorizedRequest(context);
            return;
        }

        var userId = int.Parse(userIdClaim.Value);
        var userResult = await _userService.GetUserByIdAsync(userId);

        if (!userResult.IsSuccess)
        {
            HandleUnauthorizedRequest(context);
            return;
        }

        var user = userResult.Value;

        int userRoleLevel = GetUserRoleLevel(user.Role);

        if (userRoleLevel < _minRequiredRoleLevel)
        {
            context.HttpContext.Items["AuthorizationResult"] = Result.Failure(GetCustomErrorMessage(user.Role));
            return;
        }

        context.HttpContext.Items["User"] = user;
        context.HttpContext.Items["AuthorizationResult"] = Result.Success();
    }

    private int GetUserRoleLevel(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => 4,
            UserRole.Staff => 3,
            UserRole.VerifiedUser => 2,
            UserRole.RegisteredUser => 1,
            UserRole.EmailVerifiedUser => 1,
            UserRole.PendingVerification => 1,
            UserRole.Blocked => 1,
            _ => 0,
        };
    }

    private string GetCustomErrorMessage(UserRole role)
    {
        return role switch
        {
            UserRole.RegisteredUser => "You do not have access to this resource. You need to verify your email to access this section.",
            UserRole.EmailVerifiedUser => "Your account is not fully verified. Please complete the verification process to have full access.",
            UserRole.PendingVerification => "Your verification is in progress. Access to this section is restricted.",
            UserRole.Blocked => "Your account has been blocked. You cannot access this section.",
            UserRole.Staff => "Access denied. You do not have sufficient permissions.",
            UserRole.Admin => "Only administrators can access this section.",
            _ => "Access denied. Please log in to continue."
        };
    }

    private void HandleUnauthorizedRequest(AuthorizationFilterContext context)
    {
        context.HttpContext.Items["AuthorizationResult"] = Result.Failure("Unauthorized access.");

        var isApiRequest = context.HttpContext.Request.Path.StartsWithSegments("/api");

        if (isApiRequest)
        {
            context.Result = new JsonResult(new { message = "Unauthorized" })
            {
                StatusCode = 401
            };
        }
        else
        {
            var originalUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            context.HttpContext.Response.Cookies.Append("ReturnUrl", originalUrl);
            context.Result = new RedirectToActionResult("Login", "Authentication", new { area = "" });
        }
    }
}
