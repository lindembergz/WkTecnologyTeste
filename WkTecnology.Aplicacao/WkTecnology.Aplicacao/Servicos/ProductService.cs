using Domain.Entities;
using Domain.Repositories;
using Application.DTOs;
using Application.Common;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Portifolio.Aplicacao.Common;
using Portifolio.Aplicacao.DTOs;
using Portifolio.Aplicacao.Query;
using Microsoft.VisualBasic.FileIO;
using Portifolio.Dominio.ValueObjects;
using System.Diagnostics;

namespace Portifolio.Aplicacao.Servicos
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
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

            var product = new Product(
                createProductDto.Name,
                createProductDto.Description,
                new Price(createProductDto.Price),
                createProductDto.Brand,
                createProductDto.Model,
                createProductDto.Year,
                createProductDto.Color,
                (FuelType)createProductDto.FuelType,
                createProductDto.Mileage,
                createProductDto.CategoryId
            );

            // Add images
            if (createProductDto.ImageUrls?.Any() == true)
            {
                for (int i = 0; i < createProductDto.ImageUrls.Count; i++)
                {
                    product.AddImage(createProductDto.ImageUrls[i], i == 0);
                }
            }

            // Add specifications
            if (createProductDto.Specifications?.Any() == true)
            {
                foreach (var spec in createProductDto.Specifications)
                {
                    product.AddSpecification(spec.Name, spec.Value);
                }
            }

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

            product.UpdateBasicInfo(
                updateProductDto.Name,
                updateProductDto.Description,
                new Price(updateProductDto.Price)
            );

            product.UpdateVehicleDetails(
                updateProductDto.Brand,
                updateProductDto.Model,
                updateProductDto.Year,
                updateProductDto.Color,
                (FuelType)updateProductDto.FuelType,
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
            return new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price.Amount,
                product.Price.Currency,
                product.Brand,
                product.Model,
                product.Year,
                product.Color,
                product.FuelType.ToString(),
                product.Mileage,
                product.IsActive,
                product.CategoryId,
                product.Category?.Name ?? string.Empty,
                product.CreatedAt,
                product.UpdatedAt,
                product.Images.Select(i => new ProductImageDto(i.Id, i.ImageUrl, i.IsPrimary)).ToList(),
                product.Specifications.Select(s => new ProductSpecificationDto(s.Name, s.Value)).ToList()
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
