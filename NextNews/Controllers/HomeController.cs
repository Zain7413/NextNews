using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextNews.Models;
using NextNews.Models.Database;
using NextNews.Services;
using NextNews.ViewModels;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;

namespace NextNews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IArticleService _articleService;
        private readonly IStockService _stockService;
        private readonly ICategoryService _categoryService;



        private readonly UserManager<User> _userManager;
        private readonly ISubscriptionService _subscriptionService;

        public HomeController(
            ILogger<HomeController> logger,
            IUserService userService,
            IArticleService articleService,
            ICategoryService categoryService,
            IStockService stockService,
            UserManager<User> userManager,
            ISubscriptionService subscriptionService)
        {
            _logger = logger;
            _userService = userService;
            _articleService = articleService;
            _stockService = stockService;
            _categoryService = categoryService;
            _userManager = userManager;
            _subscriptionService = subscriptionService;
        }

        //public IActionResult Index()
        //{
        //    var users = _userService.GetUsers();
        //    _logger.LogInformation("Hello!");
        //    return View();
        //}



        /* Most popular articles
           Latest articles 
           Articles by categories */



        public IActionResult Index()
        {

            string usrId = _userManager.GetUserId(HttpContext.User) ?? "";
            User usr = _userService.GetUserById(usrId);

            if (usr != null)
            {


                bool premiumSubscription = _subscriptionService.HasSubscription(usrId, "Premium"); //_subscriptionService.GetSubscriptionsAsync().Any(s => s.SubscriptionType.Name == "Premium" && s.UserId == usr.Id && s.IsActive == true);
               
                bool basicSubscription = _subscriptionService.HasSubscription(usrId, "Basic"); //_subscriptionService.GetSubscriptionsAsync().Any(s => s.SubscriptionType.Name == "Basic" && s.UserId == usr.Id && s.IsActive == true);

                if (premiumSubscription == true)
                {
                    ViewBag.SubscriptionTypeOfUser = "PremiumUser";
                }
                else if (basicSubscription == true)
                {
                    ViewBag.SubscriptionTypeOfUser = "BasicUser";
                }
                else
                {
                    ViewBag.SubscriptionTypeOfUser = "Another type of subscription";
                }

            }

            else
            {
                ViewBag.SubscriptionTypeOfUser = "User not logged in";
            }




            List<Article> allArticles = _articleService.GetArticles().ToList();
            List<Category> allCategories = _categoryService.GetCategories().ToList();

            int swedenId = allCategories.Where(a => a.Name == "Sweden").FirstOrDefault().Id;
            int localId = allCategories.Where(a => a.Name == "Local").FirstOrDefault().Id;
            int businessId = allCategories.Where(a => a.Name == "Business").FirstOrDefault().Id;
            int sportId = allCategories.Where(a => a.Name == "Sport").FirstOrDefault().Id;
            int worldId = allCategories.Where(a => a.Name == "World").FirstOrDefault().Id;
            int healthId = allCategories.Where(a => a.Name == "Health").FirstOrDefault().Id;
            int artAndCultureId = allCategories.Where(a => a.Name == "Art & Culture").FirstOrDefault().Id;
            int weatherId = allCategories.Where(a => a.Name == "Weather").FirstOrDefault().Id;
            int entertainmentId = allCategories.Where(a => a.Name == "Entertainment").FirstOrDefault().Id;

            var vm = new HomeIndexVM()
            {
                TopStoryArticle = allArticles.OrderByDescending(a => a.Views).FirstOrDefault(),
                TopStoryArticles = allArticles.OrderByDescending(a => a.Views).Take(5).ToList(),
                MostPopularArticle = allArticles.OrderByDescending(a => a.Likes).FirstOrDefault(),
                MostPopularArticles = allArticles.OrderByDescending(a => a.Likes).Take(5).ToList(),
                LatestArticle = allArticles.OrderByDescending(obj => obj.DateStamp).FirstOrDefault(),
                LatestArticles = allArticles
                    .OrderByDescending(obj => obj.DateStamp)
                    .Take(5)
                    .Select(obj => new LatestNewsViewModel()
                    {
                        Id = obj.Id,
                        HeadLine = obj.HeadLine,
                        DateStamp = obj.DateStamp,
                        ContentSummary = obj.ContentSummary,
                        ImageLink = obj.ImageLink,
                    }).ToList(),
                AllCategories = allCategories,
                ArticlesByCategorySweden = allArticles.Where(a => a.CategoryId == swedenId).OrderByDescending(a => a.DateStamp).Take(3).ToList(),
                ArticlesByCategoryLocal = allArticles.Where(a => a.CategoryId == localId).OrderByDescending(a => a.DateStamp).Take(3).ToList(),
                ArticlesByCategoryWorld = allArticles.Where(a => a.CategoryId == worldId).OrderByDescending(a => a.DateStamp).Take(4).ToList(),
                ArticlesByCategoryBusiness = allArticles.Where(a => a.CategoryId == businessId).OrderByDescending(a => a.DateStamp).Take(4).ToList(),
                ArticlesByCategorySport = allArticles.Where(a => a.CategoryId == sportId).OrderByDescending(a => a.DateStamp).Take(4).ToList(),
                ArticlesByCategoryHealth = allArticles.Where(a => a.CategoryId == healthId).OrderByDescending(a => a.DateStamp).Take(4).ToList(),
                ArticlesByCategoryWeather = allArticles.Where(a => a.CategoryId == weatherId).OrderByDescending(a => a.DateStamp).Take(4).ToList(),
                ArticlesByCategoryArtAndCulture = allArticles.Where(a => a.CategoryId == artAndCultureId).OrderByDescending(a => a.DateStamp).Take(4).ToList(),
                ArticlesByCategoryEntertainment = allArticles.Where(a => a.CategoryId == entertainmentId).OrderByDescending(a => a.DateStamp).Take(4).ToList(),
              




                EditorsChoiceArticles = allArticles.Where(a => a.IsEditorsChoice == true).OrderByDescending(a => a.DateStamp).Take(4).ToList(),

              

            };


            return View(vm);
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public async Task<IActionResult> StockReport()
        //{
        //    var stockReport = await _stockService.GetStockHttpClient("us");
        //    return View(stockReport);
        //}
        public ActionResult AboutUs()
        {
            return View();

        }

    }
}
