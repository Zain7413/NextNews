using Microsoft.AspNetCore.Mvc;
using NextNews.Models.Database;
using NextNews.Services;

namespace NextNews.Controllers
{
    public class CategoriesViewController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;

        public CategoriesViewController(IArticleService articleService, ICategoryService categoryService)
        {
            _articleService = articleService;
            _categoryService = categoryService; 
        }

        public IActionResult Index()
        {
            var categories = _articleService.GetCategories();
            return View(categories);
        }


        public IActionResult ArticlesByCategories(int categoryId)
        {
            var articles = _articleService.GetArticlesByCategory(categoryId);
            var category = _categoryService.GetCategoryById(categoryId);

            @ViewBag.CategoryName = category.Name;
            @ViewBag.BackgroundImageUrl = GetBackgroundImageUrlForCategory(category.Name);

            return View(articles);
        }
        private string GetBackgroundImageUrlForCategory(string categoryName)
        {
            // Here, map category names to their respective image URLs
            switch (categoryName.ToLower())
            {
                case "business":
                    return "/Images/Business.jpg";
                case "health":
                    return "/Images/health1.jpg";
                case "local":
                    return "/Images/Visit_PR-flygbild.jpg";
                case "art & culture":
                    return "/Images/museum-background.jpg";
                case "sport":
                    return "/Images/sport.jpg";
                case "sweden":
                    return "/Images/Sweden.jpeg";
                case "weather":
                    return "/Images/weather.jpg";
                case "world":
                    return "/Images/world.jpg";
                case "entertainment":
                    return "/Images/Entertainment.jpg";

                default:
                    return "default-background.jpg";
            }
        }

    }
}
