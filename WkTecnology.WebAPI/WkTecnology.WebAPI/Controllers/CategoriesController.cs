using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portifolio.Core;
using Portifolio.Aplicacao.DTOs;
using Portifolio.Aplicacao.Servicos;

namespace WkTecnology.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [EnableRateLimiting("DefaultPolicy")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<CategoryDto>>> GetCategories(
            [FromQuery] PagedQuery query,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _categoryService.GetCategoriesAsync(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar as categorias");
                return StatusCode(500, "Ocorreu um erro ao recuperar as categorias");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetCategory(
            [Range(1, int.MaxValue)] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);

                if (category == null)
                {
                    return NotFound($"Categoria com ID {id} não encontrada");
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar a categoria com ID: {CategoryId}", id);
                return StatusCode(500, "Ocorreu um erro ao recuperar a categoria");
            }
        }

        [HttpPost]
        [EnableRateLimiting("CreatePolicy")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> CreateCategory(
            [FromBody] CreateCategoryDto createCategoryDto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var category = await _categoryService.CreateCategoryAsync(createCategoryDto, cancellationToken);

                return CreatedAtAction(
                    nameof(GetCategory),
                    new { id = category.Id },
                    category);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar a categoria: {@Category}", createCategoryDto);
                return StatusCode(500, "Ocorreu um erro ao criar a categoria");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var updated = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto, cancellationToken);
                if (updated == null)
                    return NotFound();

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar a categoria com ID: {CategoryId}", id);
                return StatusCode(500, "Ocorreu um erro ao atualizar a categoria");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
