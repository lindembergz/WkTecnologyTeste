using Portifolio.Dominio.Entidades; 
using Portifolio.Dominio.ValueObjects; 
using Portifolio.Dominio.Repositories;    
using Portifolio.Aplicacao.DTOs;        
using Portifolio.Core;     
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Portifolio.Domain.Query; 

namespace Portifolio.Aplicacao.Servicos
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository; 
        //private readonly IDistributedCache _cache;
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDto> _updateValidator;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            //IDistributedCache cache,
            IValidator<CreateProductDto> createValidator,
            IValidator<UpdateProductDto> updateValidator)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            //_cache = cache;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQuery query, CancellationToken cancellationToken = default)
        {
            var cacheKey = GenerateCacheKey("products", query);
            PagedResult<ProductDto>? cachedResult = null; // Removed commented-out code to address S125

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


            var product = new Product(
                new ProductName(createProductDto.Name), 
                createProductDto.Description,
                createProductDto.Brand,
                createProductDto.Model,
                createProductDto.Year,
                createProductDto.Color,
                createProductDto.Mileage,
                createProductDto.CategoryId
            );

            await _productRepository.AddAsync(product, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);

            await InvalidateProductCacheAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto, CancellationToken cancellationToken = default)
        {
            await _updateValidator.ValidateAndThrowAsync(updateProductDto, cancellationToken);

            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found");

            product.UpdateBasicInfo(
                new ProductName(updateProductDto.Name), 
                updateProductDto.Description
            );

            product.UpdateVehicleDetails(
                updateProductDto.Brand,
                updateProductDto.Model,
                updateProductDto.Year,
                updateProductDto.Color,
                updateProductDto.Mileage
            );

            await _productRepository.SaveChangesAsync(cancellationToken);

            await InvalidateProductCacheAsync(id);

            return MapToDto(product);
        }

        public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return false;

            product.Deactivate(); 
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
            return new ProductDto(
                product.Id,
                product.Name.Value, 
                product.Description,
                product.Brand,
                product.Model,
                product.Year,
                product.Color,
                product.Mileage,
                product.IsActive,
                product.CategoryId,
                product.Category?.Name.Value ?? string.Empty, 
                product.CreatedAt,
                product.UpdatedAt

            );
        }

        private async Task<T?> GetFromCacheAsync<T>(string key, CancellationToken cancellationToken) where T : class
        {
            //var cached = await _cache.GetStringAsync(key, cancellationToken);
            return null;
            //return cached != null ? JsonSerializer.Deserialize<T>(cached) : null;
        }

        private async Task SetCacheAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken)
        {
           // var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
           // await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options, cancellationToken);
        }

        private async Task InvalidateProductCacheAsync(int? productId = null)
        {
            //if (productId.HasValue)
            //    await _cache.RemoveAsync($"product:{productId}");
        }

        private static string GenerateCacheKey(string prefix, object query)
        {
            var json = JsonSerializer.Serialize(query);
            var hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
            return $"{prefix}:{hash}";
        }
    }
}
