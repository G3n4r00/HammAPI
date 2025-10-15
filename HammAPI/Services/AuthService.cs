using HammAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HammAPI.Services
{

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public AuthService(IConfiguration config)
        {
            _config = config;
            var jwt = _config.GetSection("Jwt");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
            _issuer = jwt["Issuer"];
            _audience = jwt["Audience"];
            _expiryMinutes = int.Parse(jwt["ExpiryMinutes"]);
        }

        public string GenerateJwtToken(Usuario user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("firstName", user.PrimeiroNome ?? ""),
            new Claim("lastName", user.UltimoNome ?? "")
        };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
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
