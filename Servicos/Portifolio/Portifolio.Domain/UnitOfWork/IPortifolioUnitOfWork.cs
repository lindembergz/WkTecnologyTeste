using Portifolio.Dominio.Repositories;

namespace Portifolio.Dominio.UoW
{
    public interface IPortifolioUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        Task CommitAsync();
    }
}

