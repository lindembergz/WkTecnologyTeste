using UserAuth.Dominio.Entidades;

namespace UserAuth.Dominio.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByConfirmationTokenAsync(string token);
        Task AddAsync(User user);
        void Update(User user);
    }
}
