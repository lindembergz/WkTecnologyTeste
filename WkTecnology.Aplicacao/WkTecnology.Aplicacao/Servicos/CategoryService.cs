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
        // private readonly IValidator<CreateCategoryDto> _createValidator; // Adicionar se validação for implementada
        // private readonly IValidator<UpdateCategoryDto> _updateValidator; // Adicionar se validação for implementada

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(PagedQuery query, CancellationToken cancellationToken = default)
        {
            // Nota: ICategoryRepository não tem GetPagedAsync. Se for necessário, precisaria ser adicionado.
            // Por enquanto, vou buscar todas e paginar na memória, o que NÃO é ideal para grandes datasets.
            // O ideal seria o repositório suportar paginação.
            var allCategories = await _categoryRepository.GetAllAsync(cancellationToken);

            var pagedCategories = allCategories
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var categoryDtos = pagedCategories.Select(MapToDto).ToList();

            return new PagedResult<CategoryDto>
            {
                Items = categoryDtos,
                TotalCount = allCategories.Count(), // Contagem total antes da paginação em memória
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
            // TODO: Adicionar validação para updateCategoryDto
            // Ex: await _updateValidator.ValidateAndThrowAsync(updateCategoryDto, cancellationToken);

            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                return null; // Ou lançar uma exceção NotFound
            }

            if (updateCategoryDto.ParentCategoryId.HasValue && updateCategoryDto.ParentCategoryId.Value != category.ParentCategoryId)
            {
                if (updateCategoryDto.ParentCategoryId.Value == category.Id) // Não pode ser pai de si mesmo
                {
                    throw new ArgumentException("Category cannot be its own parent.");
                }
                var parentExists = await _categoryRepository.ExistsAsync(updateCategoryDto.ParentCategoryId.Value, cancellationToken);
                if (!parentExists)
                {
                    throw new ArgumentException($"Parent category with ID {updateCategoryDto.ParentCategoryId.Value} not found.");
                }
                // TODO: Verificar dependência cíclica mais profunda se necessário.
            }

            category.UpdateDetails(
                CategoryName.Create(updateCategoryDto.Name),
                updateCategoryDto.Description ?? string.Empty
            );
            // A entidade Category.cs atual não fornece um método público para alterar ParentCategoryId
            // após a instanciação. O método UpdateDetails apenas altera Name e Description.
            // Para que ParentCategoryId seja atualizável, a entidade Category precisaria ser modificada
            // para incluir um método como `ChangeParent(int? newParentCategoryId)`.
            // Fix for CS0122: "CategoryName.CategoryName(string)" é inacessível devido ao seu nível de proteção
            // The constructor for `CategoryName` is inaccessible. Based on the provided context, it seems that `CategoryName` has a static factory method `Create` that should be used to instantiate it.

            category.UpdateDetails(
                CategoryName.Create(updateCategoryDto.Name), // Use the static factory method `Create` to instantiate `CategoryName`
                updateCategoryDto.Description ?? string.Empty
            );
            // Portanto, a alteração de ParentCategoryId em updateCategoryDto não terá efeito
            // na persistência com a estrutura atual da entidade Category.
            // Se updateCategoryDto.ParentCategoryId for diferente do valor existente em category.ParentCategoryId,
            // essa mudança não será refletida.

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

            // Lógica de Soft Delete (desativar) vs Hard Delete
            // Se for soft delete:
            // category.Deactivate();
            // _categoryRepository.Update(category);
            // await _categoryRepository.SaveChangesAsync(cancellationToken);
            // return true;

            // Se for hard delete (conforme implementação atual do repositório):
            // Verificar se há subcategorias ou produtos associados antes de deletar pode ser importante.
            if (category.SubCategories.Any())
            {
                throw new InvalidOperationException("Cannot delete category with subcategories. Please delete or reassign subcategories first.");
            }
            // TODO: Verificar se há produtos associados a esta categoria.
            // var productsExist = await _productRepository.AnyProductInCategoryAsync(id, cancellationToken);
            // if (productsExist) { throw new InvalidOperationException("Cannot delete category with associated products."); }


            _categoryRepository.Delete(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        // --- Mapeamento ---
        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto(
                category.Id,
                category.Name.Value,
                category.Description,
                category.IsActive,
                category.ParentCategoryId,
                category.ParentCategory?.Name.Value, // Acesso seguro
                category.CreatedAt,
                category.UpdatedAt,
                category.SubCategories?.Select(MapToDto).ToList() ?? new List<CategoryDto>() // Mapear subcategorias recursivamente
            );
        }
    }
}
