using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace NewPlasmaDonorsAPI.Startup
{
    public static class SecurityConfig
    {
        private static readonly string[] WhiteListUrls = { "/api/user/login", "/api/user/register" };

        public static IServiceCollection AddSecurityPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Whitelist", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        var path = context.Resource as HttpContext;
                        return WhiteListUrls.Contains(path?.Request.Path.Value);
                    });
                });
            });

            return services;
        }
        public static IServiceCollection AddCorsConfig(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            return services;
        }

        public static IApplicationBuilder UseSecurity(this IApplicationBuilder app)
        {
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
