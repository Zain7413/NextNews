using NextNews.Models.Database;

namespace NextNews.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();

        List<Category> GetCategories();

        List<Category> GetCategoriesToFooter();

        Task CreateCategoryAsync(Category category);
        Task<Category> GetCategoryByIdAsync(int id);

        Category GetCategoryById(int id);

        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);

    }
}
