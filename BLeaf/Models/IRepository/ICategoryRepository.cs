using BLeaf.Models;

namespace BLeaf.Models.IRepository
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> AllCategories { get; }

        Task<Category> SaveCategory(Category category);

        Task<Category> UpdateCategory(Category category);

        Task<Category> DeleteCategory(int categoryId);

        Task<Category> FindCategoryById(int categoryId);
    }
}
