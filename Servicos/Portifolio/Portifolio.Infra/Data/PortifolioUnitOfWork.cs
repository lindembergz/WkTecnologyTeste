using System.Threading.Tasks;
using Portifolio.Dominio.Repositories;
using Portifolio.Infraestrutura.Data;
using Portifolio.Dominio.UoW;
using Portifolio.Infraestrutura.Repositories;

namespace Portifolio.Infraestrutura.UoW
{
    public class PortifolioUnitOfWork : IPortifolioUnitOfWork
    {
        private readonly PortifolioDbContext _contexto;

        public IProductRepository ProductRepository { get; }
        public ICategoryRepository CategoryRepository { get; }

        public PortifolioUnitOfWork(PortifolioDbContext contexto)
        {
            _contexto = contexto;
            ProductRepository = new ProductRepository(_contexto);
            CategoryRepository = new CategoryRepository(_contexto);
        }

        public async Task CommitAsync()
        {
            await _contexto.SaveChangesAsync();
        }
    }
}