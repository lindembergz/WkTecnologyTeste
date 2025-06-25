using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portifolio.Aplicacao.Common;
using Portifolio.Aplicacao.DTOs;
using Portifolio.Aplicacao.Query;
using Portifolio.Aplicacao.Servicos;
using System.ComponentModel.DataAnnotations;

namespace WkTecnology.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [EnableRateLimiting("DefaultPolicy")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paginated list of products with optional filtering
        /// </summary>
        /// <param name="query">Search and filter parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated product list</returns>
        [HttpGet]
        [EnableRateLimiting("SearchPolicy")]
        [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
            [FromQuery] ProductQuery query,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (query.PageSize > 100)
                {
                    return BadRequest("Page size cannot exceed 100 items");
                }

                var result = await _productService.GetProductsAsync(query, cancellationToken);

                // Add pagination headers
                Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
                Response.Headers.Add("X-Page", result.Page.ToString());
                Response.Headers.Add("X-Page-Size", result.PageSize.ToString());
                Response.Headers.Add("X-Total-Pages", result.TotalPages.ToString());

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products with query: {@Query}", query);
                return StatusCode(500, "An error occurred while retrieving products");
            }
        }

        /// <summary>
        /// Retrieves a specific product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Product details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProduct(
            [Range(1, int.MaxValue)] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id, cancellationToken);

                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while retrieving the product");
            }
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="createProductDto">Product creation data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created product</returns>
        [HttpPost]
        [EnableRateLimiting("CreatePolicy")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ProductDto>> CreateProduct(
            [FromBody] CreateProductDto createProductDto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _productService.CreateProductAsync(createProductDto, cancellationToken);

                return CreatedAtAction(
                    nameof(GetProduct),
                    new { id = product.Id },
                    product);
            }
            catch (ValidationException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {@Product}", createProductDto);
                return StatusCode(500, "An error occurred while creating the product");
            }
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Product update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated product</returns>
        [HttpPut("{id:int}")]
        [EnableRateLimiting("UpdatePolicy")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ProductDto>> UpdateProduct(
            [Range(1, int.MaxValue)] int id,
            [FromBody] UpdateProductDto updateProductDto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDto, cancellationToken);
                return Ok(product);
            }
            catch (ValidationException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while updating the product");
            }
        }

        /// <summary>
        /// Soft deletes a product (deactivates it)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success indicator</returns>
        [HttpDelete("{id:int}")]
        [EnableRateLimiting("DeletePolicy")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(
            [Range(1, int.MaxValue)] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _productService.DeleteProductAsync(id, cancellationToken);

                if (!deleted)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while deleting the product");
            }
        }

        /// <summary>
        /// Activates a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success indicator</returns>
        [HttpPatch("{id:int}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateProduct(
            [Range(1, int.MaxValue)] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var activated = await _productService.ActivateProductAsync(id, cancellationToken);

                if (!activated)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating product with ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while activating the product");
            }
        }

        /// <summary>
        /// Deactivates a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success indicator</returns>
        [HttpPatch("{id:int}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateProduct(
            [Range(1, int.MaxValue)] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var deactivated = await _productService.DeactivateProductAsync(id, cancellationToken);

                if (!deactivated)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating product with ID: {ProductId}", id);
                return StatusCode(500, "An error occurred while deactivating the product");
            }
        }
    }
}
