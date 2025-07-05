using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portifolio.Core;
using Portifolio.Aplicacao.DTOs;
using Portifolio.Aplicacao.Servicos;
using System.ComponentModel.DataAnnotations;
using Portifolio.Domain.Query;

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
                    return BadRequest("O tamanho da página não pode exceder 100 itens");
                }

                var result = await _productService.GetProductsAsync(query, cancellationToken);

                Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
                Response.Headers.Add("X-Page", result.Page.ToString());
                Response.Headers.Add("X-Page-Size", result.PageSize.ToString());
                Response.Headers.Add("X-Total-Pages", result.TotalPages.ToString());

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar os produtos com a consulta: {@Query}", query);
                return StatusCode(500, "Ocorreu um erro ao recuperar os produtos");
            }
        }

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
                    return NotFound($"Produto com ID {id} não encontrado");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar o produto com ID: {ProductId}", id);
                return StatusCode(500, "Ocorreu um erro ao recuperar o produto");
            }
        }

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
                return BadRequest(ex.Message); // já está traduzido na exceção
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar o produto: {@Product}", createProductDto);
                return StatusCode(500, "Ocorreu um erro ao criar o produto");
            }
        }

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
                return BadRequest(ex.Message); // já está traduzido na exceção
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar o produto com ID: {ProductId}", id);
                return StatusCode(500, "Ocorreu um erro ao atualizar o produto");
            }
        }

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
                    return NotFound($"Produto com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o produto com ID: {ProductId}", id);
                return StatusCode(500, "Ocorreu um erro ao excluir o produto");
            }
        }

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
                    return NotFound($"Produto com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ativar o produto com ID: {ProductId}", id);
                return StatusCode(500, "Ocorreu um erro ao ativar o produto");
            }
        }

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
                    return NotFound($"Produto com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar o produto com ID: {ProductId}", id);
                return StatusCode(500, "Ocorreu um erro ao desativar o produto");
            }
        }
    }
}
