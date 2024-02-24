using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NextNews.Data;
using NextNews.Models.Database;

namespace NextNews.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }


        //get category
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }


        public List<Category> GetCategories()
        {
            var objList = _context.Categories.ToList();

            return objList;
        }


        public List<Category> GetCategoriesToFooter()
        {
            var objList = _context.Categories.Take(5).ToList();

            return objList;
        }


        //create category
        public async Task CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }


        //details category
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }


        public Category GetCategoryById(int id)
        {
            return _context.Categories.Find(id);
        }


        //Update category
        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }


        //delete category
        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }



    }
}
