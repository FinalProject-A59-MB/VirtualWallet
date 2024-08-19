using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.DATA.Services;
using VirtualWallet.DATA.Services.Contracts;

namespace VirtualWallet.WEB.Middlewares
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authService = context.RequestServices.GetRequiredService<IAuthService>();
            var userService = context.RequestServices.GetRequiredService<IUserService>();

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token == null)
            {
                token = context.Request.Cookies["jwt"];
            }

            if (token != null && authService.ValidateToken(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken != null)
                {
                    var usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)
                                        ?? jwtToken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")
                                        ?? jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

                    if (usernameClaim != null)
                    {
                        var username = usernameClaim.Value;
                        var user = await userService.GetUserByUsernameAsync(username);

                        if (user != null)
                        {
                            var userProfile = await userService.GetUserProfileAsync(user.Id);
                            context.Items["CurrentUser"] = user;
                            context.Items["UserProfile"] = userProfile;
                        }
                    }
                }
            }

            await _next(context);
        }
    }


}
