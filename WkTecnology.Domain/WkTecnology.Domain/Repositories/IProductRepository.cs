using Portifolio.Core;
using Portifolio.Domain.Query;
using Portifolio.Dominio.Entidades;   // Para ProductQuery

namespace Portifolio.Dominio.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PagedResult<Product>> GetPagedAsync(ProductQuery query, CancellationToken cancellationToken = default);
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        void Update(Product product); // EF Core rastreia a entidade. O serviço chamará SaveChangesAsync.
        void Delete(Product product); // O serviço chamará SaveChangesAsync.
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // Para persistir as mudanças
        // Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default); // Adicionar se necessário
    }
}
