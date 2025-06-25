using Portifolio.Dominio.Entidades; // Para Category (se usado diretamente)
using Portifolio.Dominio.ValueObjects; // Para Product, ProductName, Price (se existisse), FuelType (se existisse)
using Portifolio.Dominio.Repositories;    // Para IProductRepository, ICategoryRepository
using Portifolio.Aplicacao.DTOs;        // Para ProductDto, CreateProductDto, UpdateProductDto
using Portifolio.Aplicacao.Common;      // Para PagedResult
using Portifolio.Aplicacao.Query;         // Para ProductQuery
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
// using Microsoft.VisualBasic.FileIO; // Parece não utilizado
using System.Diagnostics; // Parece não utilizado diretamente, talvez por alguma dependência transitiva

namespace Portifolio.Aplicacao.Servicos
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository; // Usado para categoryExists
        private readonly IDistributedCache _cache;
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDto> _updateValidator;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IDistributedCache cache,
            IValidator<CreateProductDto> createValidator,
            IValidator<UpdateProductDto> updateValidator)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _cache = cache;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQuery query, CancellationToken cancellationToken = default)
        {
            var cacheKey = GenerateCacheKey("products", query);
            var cachedResult = await GetFromCacheAsync<PagedResult<ProductDto>>(cacheKey, cancellationToken);

            if (cachedResult != null)
                return cachedResult;

            var result = await _productRepository.GetPagedAsync(query, cancellationToken);
            var productDtos = result.Items.Select(MapToDto).ToList();

            var pagedResult = new PagedResult<ProductDto>
            {
                Items = productDtos,
                TotalCount = result.TotalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };

            await SetCacheAsync(cacheKey, pagedResult, TimeSpan.FromMinutes(5), cancellationToken);
            return pagedResult;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"product:{id}";
            var cachedProduct = await GetFromCacheAsync<ProductDto>(cacheKey, cancellationToken);

            if (cachedProduct != null)
                return cachedProduct;

            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return null;

            var productDto = MapToDto(product);
            await SetCacheAsync(cacheKey, productDto, TimeSpan.FromMinutes(10), cancellationToken);

            return productDto;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken = default)
        {
            await _createValidator.ValidateAndThrowAsync(createProductDto, cancellationToken);

            var categoryExists = await _categoryRepository.ExistsAsync(createProductDto.CategoryId, cancellationToken);
            if (!categoryExists)
                throw new ArgumentException($"Category with ID {createProductDto.CategoryId} does not exist");

            // Ajustado para o construtor de Product.cs:
            // public Product(ProductName name, string description, string brand, string model, int year, string color, int mileage, int categoryId)
            // createProductDto.Name precisará ser encapsulado em ProductName se o validador/controller não o fizer.
            // Por enquanto, assumindo que CreateProductDto terá ProductName ou que o construtor de Product aceita string para o nome.
            // A entidade Product tem `public ProductName Name { get; private set; }` e construtor `Product(ProductName name, ...)`
            // Então, precisamos de `new ProductName(createProductDto.Name)`
            var product = new Product(
                new ProductName(createProductDto.Name), // Ajustado para ProductName
                createProductDto.Description,
                // Removido: new Price(createProductDto.Price),
                createProductDto.Brand,
                createProductDto.Model,
                createProductDto.Year,
                createProductDto.Color,
                // Removido: (FuelType)createProductDto.FuelType,
                createProductDto.Mileage,
                createProductDto.CategoryId
            );

            // Removido: Lógica de Add images
            // Removido: Lógica de Add specifications

            await _productRepository.AddAsync(product, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            await InvalidateProductCacheAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto, CancellationToken cancellationToken = default)
        {
            await _updateValidator.ValidateAndThrowAsync(updateProductDto, cancellationToken);

            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found");

            // Ajustado para UpdateBasicInfo(ProductName name, string description)
            product.UpdateBasicInfo(
                new ProductName(updateProductDto.Name), // Ajustado para ProductName
                updateProductDto.Description
                // Removido: new Price(updateProductDto.Price)
            );

            // Ajustado para UpdateVehicleDetails(string brand, string model, int year, string color, int mileage)
            product.UpdateVehicleDetails(
                updateProductDto.Brand,
                updateProductDto.Model,
                updateProductDto.Year,
                updateProductDto.Color,
                // Removido: (FuelType)updateProductDto.FuelType,
                updateProductDto.Mileage
            );

            await _productRepository.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            await InvalidateProductCacheAsync(id);

            return MapToDto(product);
        }

        public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return false;

            product.Deactivate(); // Soft delete
            await _productRepository.SaveChangesAsync(cancellationToken);

            await InvalidateProductCacheAsync(id);
            return true;
        }

        public async Task<bool> ActivateProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return false;

            product.Activate();
            await _productRepository.SaveChangesAsync(cancellationToken);

            await InvalidateProductCacheAsync(id);
            return true;
        }

        public async Task<bool> DeactivateProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return false;

            product.Deactivate();
            await _productRepository.SaveChangesAsync(cancellationToken);

            await InvalidateProductCacheAsync(id);
            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            // Ajustado para refletir os campos reais de Product.cs e o ProductDto simplificado
            return new ProductDto(
                product.Id,
                product.Name.Value, // ProductName é um ValueObject, pegamos o .Value
                product.Description,
                // Removido: product.Price.Amount,
                // Removido: product.Price.Currency,
                product.Brand,
                product.Model,
                product.Year,
                product.Color,
                // Removido: product.FuelType.ToString(),
                product.Mileage,
                product.IsActive,
                product.CategoryId,
                product.Category?.Name.Value ?? string.Empty, // Category.Name é CategoryName
                product.CreatedAt,
                product.UpdatedAt
                // Removido: product.Images...
                // Removido: product.Specifications...
            );
        }

        private async Task<T?> GetFromCacheAsync<T>(string key, CancellationToken cancellationToken) where T : class
        {
            var cached = await _cache.GetStringAsync(key, cancellationToken);
            return cached != null ? JsonSerializer.Deserialize<T>(cached) : null;
        }

        private async Task SetCacheAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken)
        {
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options, cancellationToken);
        }

        private async Task InvalidateProductCacheAsync(int? productId = null)
        {
            if (productId.HasValue)
                await _cache.RemoveAsync($"product:{productId}");

            // In production, use Redis pattern matching to clear all product list caches
            // For simplicity, we're just clearing specific keys
        }

        private static string GenerateCacheKey(string prefix, object query)
        {
            var json = JsonSerializer.Serialize(query);
            var hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
            return $"{prefix}:{hash}";
        }
    }
}
