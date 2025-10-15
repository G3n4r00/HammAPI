using HammAPI.Data;
using HammAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HammAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly HammAPIDbContext _db;
        public UserRepository(HammAPIDbContext db) => _db = db;

        public async Task<Usuario?> GetByEmailAsync(string email) =>
            await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<Usuario?> GetByIdAsync(Guid id) =>
            await _db.Usuarios.FindAsync(id);

        public async Task AddAsync(Usuario usuario)
        {
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
        }
    }
}
