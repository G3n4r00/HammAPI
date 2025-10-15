using HammAPI.Models;

namespace HammAPI.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(Usuario user);
        bool VerifyPassword(string senha, string senhaHash);
        string HashPassword(string senha);
    }
}
