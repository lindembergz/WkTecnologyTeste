using Portifolio.Core;
using Portifolio.Aplicacao.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace Portifolio.Aplicacao.Servicos
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetCategoriesAsync(PagedQuery query, CancellationToken cancellationToken = default);
        Task<CategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto, CancellationToken cancellationToken = default);
        Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken = default); // UpdateCategoryDto precisará ser criado
        Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
        // Task<bool> ActivateCategoryAsync(int id, CancellationToken cancellationToken = default); // Adicionar se necessário
        // Task<bool> DeactivateCategoryAsync(int id, CancellationToken cancellationToken = default); // Adicionar se necessário
    }
}
