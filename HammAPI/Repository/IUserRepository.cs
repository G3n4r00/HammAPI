using HammAPI.Models;

namespace HammAPI.Repository
{
    public interface IUserRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByIdAsync(Guid id);
        Task AddAsync(Usuario usuario);
    }
}
