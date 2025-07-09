using UserAuth.Dominio.Repositories;

namespace UserAuth.Dominio.UoW
{
    public interface IUserUnitOfWork
    {
        IUserRepository UserRepository { get; }
        Task CommitAsync();
    }
}

