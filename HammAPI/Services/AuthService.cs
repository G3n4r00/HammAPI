using HammAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HammAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public AuthService(IConfiguration config)
        {
            var jwt = config.GetSection("Jwt");

            var key = jwt["Key"];
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];
            var expiry = jwt["ExpiryMinutes"];

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            _issuer = issuer;
            _audience = audience;
            _expiryMinutes = int.Parse(expiry ?? "60");
        }

        public string GenerateJwtToken(Usuario user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("firstName", user.PrimeiroNome ?? ""),
                new Claim("lastName", user.UltimoNome ?? "")
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool VerifyPassword(string senha, string senhaHash)
        {
            return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
        }

        public string HashPassword(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }
    }
}
