using Microsoft.AspNetCore.Mvc;
using NextNews.Services;

namespace NextNews.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        // Injections
        private readonly ICategoryService _categoryService;

        // Constructor
        public FooterViewComponent(ICategoryService categoryService) 
        {
            _categoryService = categoryService;
        }    


        public IViewComponentResult Invoke()
        {

            var objList = _categoryService.GetCategoriesToFooter();

            return View(objList);
        }





    }
}
