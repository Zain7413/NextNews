using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextNews.Services;

namespace NextNews.ViewComponents
{
    public class HomeViewComponent : ViewComponent
    {
        //Injections
        private readonly ICategoryService _categoryService;
        private readonly IArticleService _articleService;

        // Constructor
        public HomeViewComponent(ICategoryService categoryService, IArticleService articleService)
        {
            _categoryService = categoryService;
            _articleService = articleService;
        }

        [Authorize(Roles = "Premium")]
        public IViewComponentResult Invoke()
        {
            var objList = _articleService.GetArticles();

            return View(objList);
        }


    }
}
