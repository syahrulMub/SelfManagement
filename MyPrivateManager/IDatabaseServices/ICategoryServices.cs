using MyPrivateManager.Models;
namespace IDatabaseServices;

public interface ICategoryServices
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int? categoryId);
    Task<bool> CreateCategoryAsync(Category category);
    Task<bool> UpdateCategoryAsync(int categoryId, Category category);
    Task<bool> DeleteCategoryAsync(int categoryId);
}