using Portifolio.Dominio.Entidades;
using WkTecnology.Core; 

namespace Portifolio.Dominio.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetSubcategoriesAsync(int parentId, CancellationToken cancellationToken = default);
        Task AddAsync(Category category, CancellationToken cancellationToken = default);
        void Update(Category category); 
        void Delete(Category category); 
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); 
    }
}
