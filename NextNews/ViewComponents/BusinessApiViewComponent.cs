using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextNews.Models.Database;
using NextNews.Services;

namespace NextNews.ViewComponents
{
    public class BusinessApiViewComponent : ViewComponent
    {
        private readonly IStockService _stockService;

        public BusinessApiViewComponent(IStockService stockService)
        {
            _stockService = stockService;       
        }



        [Authorize(Roles = "Premium")]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {

                var api = await _stockService.GetStockHttpClient("us");
                if (api == null)
                {
                    return View("Default", new Stock { ErrorMessage = "No data available" });
                }
                return View("Default", api);
              
            }
            catch (HttpRequestException ex) 
            {
                return View("Default", new Stock { ErrorMessage = "Error fetching stock data." });
            }
        }


    }
}
