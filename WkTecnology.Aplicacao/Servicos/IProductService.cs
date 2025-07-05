using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portifolio.Core;
using Portifolio.Aplicacao.DTOs;
using Portifolio.Domain.Query;

namespace Portifolio.Aplicacao.Servicos
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(ProductQuery query, CancellationToken cancellationToken = default);
        Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken = default);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto, CancellationToken cancellationToken = default);
        Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ActivateProductAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> DeactivateProductAsync(int id, CancellationToken cancellationToken = default);
    }
}
