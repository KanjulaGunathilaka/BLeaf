using BLeaf.Data;
using BLeaf.Models.IRepository;

namespace BLeaf.Models.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoryRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<Category> AllCategories => _applicationDbContext.Categories.OrderBy(p => p.Name);

        public async Task<Category> SaveCategory(Category category)
        {
            _applicationDbContext.Categories.Add(category);
            await _applicationDbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategory(Category category)
        {
            _applicationDbContext.Categories.Update(category);
            await _applicationDbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category> DeleteCategory(int categoryId)
        {
            var category = await _applicationDbContext.Categories.FindAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }

            _applicationDbContext.Categories.Remove(category);
            await _applicationDbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category> FindCategoryById(int categoryId)
        {
            return await _applicationDbContext.Categories.FindAsync(categoryId);
        }
    }
}