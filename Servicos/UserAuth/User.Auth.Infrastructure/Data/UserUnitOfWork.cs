using System.Threading.Tasks;
using UserAuth.Infraestrutura.Data;
using UserAuth.Dominio.Repositories;
using UserAuth.Infraestrutura.Repositories;
using UserAuth.Dominio.UoW;


namespace UserAuth.Infraestrutura.UoW
{
    public class UserUnitOfWork : IUserUnitOfWork
    {
        private readonly UserDbContext _contexto;

        public IUserRepository UserRepository { get; }

        public UserUnitOfWork(UserDbContext contexto)
        {
            _contexto = contexto;
            UserRepository = new UserRepository(_contexto);
        }

        public async Task CommitAsync()
        {
            await _contexto.SaveChangesAsync();
        }
    }
}