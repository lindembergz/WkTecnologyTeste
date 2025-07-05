using Portifolio.Core;
using Portifolio.Aplicacao.DTOs;
using Portifolio.Dominio.Entidades;
using Portifolio.Dominio.Repositories;
using Portifolio.Dominio.ValueObjects; // Para CategoryName
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic; // Para List

namespace Portifolio.Aplicacao.Servicos
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(PagedQuery query, CancellationToken cancellationToken = default)
        {
            //Nota: ICategoryRepository não tem GetPagedAsync. Se for necessário, precisaria ser adicionado.
            //No cenário de Categoria, por enquanto, vou buscar todas e paginar na memória, o que não é ideal para grandes datasets,
            //o que é o caso.

            var allCategories = await _categoryRepository.GetAllAsync(cancellationToken);

            var pagedCategories = allCategories
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var categoryDtos = pagedCategories.Select(MapToDto).ToList();

            return new PagedResult<CategoryDto>
            {
                Items = categoryDtos,
                TotalCount = allCategories.Count(), 
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            return category == null ? null : MapToDto(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto, CancellationToken cancellationToken = default)
        {
            int? parentCategoryId = createCategoryDto.ParentCategoryId;
            if (parentCategoryId == 0)
                parentCategoryId = null;

            if (parentCategoryId.HasValue)
            {
                var parentExists = await _categoryRepository.ExistsAsync(parentCategoryId.Value, cancellationToken);
                if (!parentExists)
                {
                    throw new ArgumentException($"Parent category with ID {parentCategoryId.Value} not found.");
                }
            }

            var category = new Category(
                CategoryName.Create(createCategoryDto.Name),
                createCategoryDto.Description ?? string.Empty,
                parentCategoryId
            );

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return MapToDto(category);
        }


        public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                return null; 
            }

            if (updateCategoryDto.ParentCategoryId.HasValue && updateCategoryDto.ParentCategoryId.Value != category.ParentCategoryId)
            {
                if (updateCategoryDto.ParentCategoryId.Value == category.Id) 
                {
                    throw new ArgumentException("Category cannot be its own parent.");
                }
                var parentExists = await _categoryRepository.ExistsAsync(updateCategoryDto.ParentCategoryId.Value, cancellationToken);
                if (!parentExists)
                {
                    throw new ArgumentException($"Parent category with ID {updateCategoryDto.ParentCategoryId.Value} not found.");
                }
            }

            category.UpdateDetails(
                CategoryName.Create(updateCategoryDto.Name),
                updateCategoryDto.Description ?? string.Empty
            );

            category.UpdateDetails(
                CategoryName.Create(updateCategoryDto.Name), 
                updateCategoryDto.Description ?? string.Empty
            );

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return MapToDto(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                return false;
            }

            if (category.SubCategories.Any())
            {
                throw new InvalidOperationException("Cannot delete category with subcategories. Please delete or reassign subcategories first.");
            }

            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return true;
        }


        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto(
                category.Id,
                category.Name.Value,
                category.Description,
                category.IsActive,
                category.ParentCategoryId,
                category.ParentCategory?.Name.Value, 
                category.CreatedAt,
                category.UpdatedAt,
                category.SubCategories?.Select(MapToDto).ToList() ?? new List<CategoryDto>() 
            );
        }
    }
}
