using Portifolio.Dominio.Entidades;
using Portifolio.Aplicacao.Common; // Para PagedResult e PagedQuery, se necessário

namespace Portifolio.Dominio.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        // Task<PagedResult<Category>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default); // Adicionar se necessário para Category
        Task<IEnumerable<Category>> GetSubcategoriesAsync(int parentId, CancellationToken cancellationToken = default);
        Task AddAsync(Category category, CancellationToken cancellationToken = default);
        void Update(Category category); // EF Core rastreia a entidade. O serviço chamará SaveChangesAsync.
        void Delete(Category category); // O serviço chamará SaveChangesAsync.
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // Para persistir as mudanças
    }
}
