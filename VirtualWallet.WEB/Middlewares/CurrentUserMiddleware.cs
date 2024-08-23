using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VirtualWallet.BUSINESS.Services.Contracts;
using VirtualWallet.DATA.Models;
using VirtualWallet.BUSINESS.Results;
using Microsoft.AspNetCore.Http;
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

            if (token != null)
            {
                var validateTokenResult = authService.ValidateToken(token);

                if (validateTokenResult.IsSuccess)
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

                            var userResult = await userService.GetUserByUsernameAsync(username);

                            if (userResult.IsSuccess)
                            {
                                var user = userResult.Value;

                                var userProfileResult = await userService.GetUserProfileAsync(user.Id);
                                if (userProfileResult.IsSuccess)
                                {
                                    context.Items["CurrentUser"] = user;
                                    context.Items["UserProfile"] = userProfileResult.Value;
                                }
                                
                            }
                            
                        }
                    }
                }
                
            }

            await _next(context);
        }
    }
}
