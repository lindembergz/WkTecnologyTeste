using WkTecnology.Core;
using Portifolio.Domain.Query;
using Portifolio.Dominio.Entidades;   

namespace Portifolio.Dominio.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResult<Product>> GetPagedAsync(ProductQuery query, CancellationToken cancellationToken = default);
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        void Update(Product product); 
        void Delete(Product product); 
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); 
    }
}
