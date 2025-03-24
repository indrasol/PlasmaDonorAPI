using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NewPlasmaDonorsAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace NewPlasmaDonorsAPI.Middleware
{
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring(7);
                try
                {
                    var jwtService = context.RequestServices.GetRequiredService<IJwtService>();
                    var username = jwtService.GetUsernameFromToken(token);
                    if (!string.IsNullOrEmpty(username))
                    {
                        context.User = jwtService.GetPrincipalFromToken(token);
                    }
                }
                catch
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
            await _next(context);
        }
        
    }
}