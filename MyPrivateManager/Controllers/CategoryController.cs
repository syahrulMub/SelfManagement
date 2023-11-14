using IDatabaseServices;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryServices _services;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryServices services, ILogger<CategoryController> logger)
    {
        _services = services;
        _logger = logger;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        try
        {
            var categories = await _services.GetCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<Category>> GetCategory(int categoryId)
    {
        try
        {
            var category = await _services.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid input for CreateCategory");
                return BadRequest(ModelState);
            }

            var success = await _services.CreateCategoryAsync(category);

            if (success)
            {
                return CreatedAtAction(nameof(GetCategory), new { categoryId = category.CategoryId }, category);
            }
            else
            {
                _logger.LogError("Failed to create category");
                return StatusCode(500, "Failed to create category");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut("{categoryId}")]
    public async Task<ActionResult<Category>> UpdateCategory(int categoryId, [FromBody] Category category)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid input for UpdateCategory");
                return BadRequest(ModelState);
            }

            var updatedCategory = await _services.UpdateCategoryAsync(categoryId, category);

            if (updatedCategory == false)
            {
                return NotFound();
            }

            return Ok(updatedCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpDelete("{categoryId}")]
    public async Task<ActionResult> DeleteCategory(int categoryId)
    {
        try
        {
            var result = await _services.DeleteCategoryAsync(categoryId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            return StatusCode(500, "Internal Server Error");
        }
    }
}
