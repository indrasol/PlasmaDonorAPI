using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewPlasmaDonorsAPI.Services
{
    public interface IJwtService
    {
        string GenerateToken(string username);
        string? GetUsernameFromToken(string token);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly string _jwtSecret;

        public JwtService(string jwtSecret)
        {
            _jwtSecret = jwtSecret;
        }

        public string GenerateToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret.PadRight(32, '0'));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string? GetUsernameFromToken(string token)
        {
            var principal = GetPrincipalFromToken(token);
            return principal.Identity?.Name;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out _);
            return principal;
        }
    }
}
