using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portifolio.Aplicacao.Common;
using Portifolio.Aplicacao.DTOs;

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
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, "An error occurred while retrieving categories");
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
                    return NotFound($"Category with ID {id} not found");
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID: {CategoryId}", id);
                return StatusCode(500, "An error occurred while retrieving the category");
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
                _logger.LogError(ex, "Error creating category: {@Category}", createCategoryDto);
                return StatusCode(500, "An error occurred while creating the category");
            }
        }
    }
}
