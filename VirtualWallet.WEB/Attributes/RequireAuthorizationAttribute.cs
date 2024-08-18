using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using VirtualWallet.DATA.Repositories.Contracts;
using VirtualWallet.BUSINESS.Services.Contracts;

public class RequireAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    private readonly bool _requireAdmin;
    private readonly bool _checkIfBlocked;
    private IAuthService _authService;
    private IUserRepository _userRepository;

    public RequireAuthorizationAttribute(bool requireAdmin = false, bool checkIfBlocked = false)
    {
        _requireAdmin = requireAdmin;
        _checkIfBlocked = checkIfBlocked;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        _authService = context.HttpContext.RequestServices.GetService<IAuthService>();
        _userRepository = context.HttpContext.RequestServices.GetService<IUserRepository>();
        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token == null)
        {
            token = context.HttpContext.Request.Cookies["jwt"];
        }

        if (token == null || !_authService.ValidateToken(token))
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
        var isAdminClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin");

        if (userIdClaim == null)
        {
            HandleUnauthorizedRequest(context);
            return;
        }

        var userId = int.Parse(userIdClaim.Value);
        var isAdmin = isAdminClaim != null;

        var user = _userRepository.GetUserByIdAsync(userId);

        if (user == null || (_requireAdmin && !isAdmin))
        {
            HandleUnauthorizedRequest(context);
            return;
        }

        if (_checkIfBlocked)//TODO blocked logic
        {
            HandleBlockedUserRequest(context);
            return;
        }

        context.HttpContext.Items["User"] = user;
    }

    private void HandleUnauthorizedRequest(AuthorizationFilterContext context)
    {
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

    private void HandleBlockedUserRequest(AuthorizationFilterContext context)
    {
        var isApiRequest = context.HttpContext.Request.Path.StartsWithSegments("/api");

        if (isApiRequest)
        {
            context.Result = new JsonResult(new { message = "Your account has been blocked. You cannot add comments or create posts." })
            {
                StatusCode = 403
            };
        }
        else
        {
            throw new UnauthorizedAccessException("Your account has been blocked. You cannot add comments or create posts.");
        }
    }
}
