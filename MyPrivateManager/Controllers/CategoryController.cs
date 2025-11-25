using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryServices _categoryServices;
    private readonly ILogger<CategoryController> _logger;
    private readonly UserManager<User> _userManager;

    public CategoryController(ICategoryServices services, ILogger<CategoryController> logger, UserManager<User> userManager)
    {
        _categoryServices = services;
        _logger = logger;
        _userManager = userManager;
    }
    [HttpGet("/Category/Categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var categories = await _categoryServices.GetCategoriesAsync();
                var userCategories = categories.Where(i => i.UserId == userId);
                return Ok(categories);
            }
            else
            {
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return View("Error");
        }
    }

    [HttpGet("/Category/GetCategory/{categoryId}")]
    public async Task<IActionResult> GetCategory(int categoryId)
    {
        try
        {
            var category = await _categoryServices.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                _logger.LogError("category not found");
                return NotFound();
            }
            _logger.LogInformation("success get caetegory data");
            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category");
            return View("Error");
        }
    }

    [HttpPost("/Category/CreateCategory")]
    public async Task<IActionResult> CreateCategory(Category category)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                category.UserId = userId;
                var success = await _categoryServices.CreateCategoryAsync(category);
                _logger.LogInformation("success create category");
                return Ok();
            }
            else
            {
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return View("Error");
        }
    }

    [HttpPost("/Category/UpdateCategory/{categoryId}")]
    public async Task<IActionResult> UpdateCategory(int categoryId, Category category)
    {
        try
        {
            await _categoryServices.UpdateCategoryAsync(categoryId, category);
            _logger.LogInformation("success update category");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            return View("Error");
        }
    }

    [HttpDelete("/Category/DeleteCategory/{categoryId}")]
    public async Task<ActionResult> DeleteCategory(int categoryId)
    {
        try
        {
            await _categoryServices.DeleteCategoryAsync(categoryId);
            _logger.LogInformation("succes delete gategory");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            return View("Error");
        }
    }
}
