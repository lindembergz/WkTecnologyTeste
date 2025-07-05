using System.Threading.Tasks;
using Portifolio.Dominio.Repositories;
using Portifolio.Infraestrutura.Data;
using Portifolio.Infraestrutura.Repositories;

namespace ICiProvaCandidato.Dominio.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _contexto;

        public IProductRepository ProductRepository { get; }
        public ICategoryRepository CategoryRepository { get; }

        public UnitOfWork(ApplicationDbContext contexto)
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