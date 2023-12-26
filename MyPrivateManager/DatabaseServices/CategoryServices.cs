using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace DatabaseServices
{
    public class CategoryServices : ICategoryServices
    {
        private readonly DatabaseContext _dbContext;

        public CategoryServices(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _dbContext.Categories
                            .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int? categoryId)
        {
            return await _dbContext.Categories
                    .Where(c => c.CategoryId == categoryId)
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, Category category)
        {
            var existingCategory = await _dbContext.Categories.FirstOrDefaultAsync(i => i.CategoryId == categoryId);

            if (existingCategory != null)
            {
                existingCategory.CategoryName = category.CategoryName;
                _dbContext.Categories.Update(existingCategory);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(i => i.CategoryId == categoryId);

            if (category != null)
            {
                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
