using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryServices _categoryServices;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryServices services, ILogger<CategoryController> logger)
    {
        _categoryServices = services;
        _logger = logger;
    }
    [HttpGet("/categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _categoryServices.GetCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return View("Error");
        }
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetCategory(int categoryId)
    {
        try
        {
            var category = await _categoryServices.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] Category category)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid input for CreateCategory");
                return BadRequest(ModelState);
            }

            var success = await _categoryServices.CreateCategoryAsync(category);

            if (success)
            {
                return Ok();
            }
            else
            {
                _logger.LogError("Failed to create category");
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return View("Error");
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

            var updatedCategory = await _categoryServices.UpdateCategoryAsync(categoryId, category);

            if (updatedCategory == false)
            {
                return NotFound();
            }

            return Ok(updatedCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            return View("Error");
        }
    }

    [HttpDelete("{categoryId}")]
    public async Task<ActionResult> DeleteCategory(int categoryId)
    {
        try
        {
            var result = await _categoryServices.DeleteCategoryAsync(categoryId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            return View("Error");
        }
    }
}
