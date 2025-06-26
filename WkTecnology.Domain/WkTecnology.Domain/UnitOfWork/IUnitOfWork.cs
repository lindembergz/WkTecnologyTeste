using Portifolio.Dominio.Repositories;

namespace ICiProvaCandidato.Dominio.UoW
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        Task CommitAsync();
    }
}

