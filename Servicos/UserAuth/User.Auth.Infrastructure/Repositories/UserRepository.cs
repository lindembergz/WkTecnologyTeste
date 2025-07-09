using Microsoft.EntityFrameworkCore;
using UserAuth.Dominio.Entidades;
using UserAuth.Dominio.Repositories;
using UserAuth.Infraestrutura.Data;

namespace UserAuth.Infraestrutura.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetByConfirmationTokenAsync(string token)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.EmailConfirmationToken == token);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }
    }
}
