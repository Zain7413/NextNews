using Microsoft.AspNetCore.Mvc;

namespace NextNews.Controllers
{
    public class CurrencyRatesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
