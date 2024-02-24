using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextNews.Services;
using NextNews.ViewModels;

namespace NextNews.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;
        private  ISubscriptionService _subscriptionService;
        private readonly IStatisticService _statisticService;

        public StatisticsController(IArticleService articleService , IUserService userService , ISubscriptionService subscriptionService,IStatisticService statisticService)
        {
            _articleService = articleService;
            _userService = userService;
            _subscriptionService = subscriptionService;
            _statisticService = statisticService;
        }


        [Authorize(Roles = "Admin, Editor")]
        public async Task <IActionResult> Index()
        {
            // Fetching user count and article count from services
            int userCount =  _userService.GetUsers().Count();
            int articleCount = _articleService.GetArticles().Count();

            // Fetching basic subscription users count
            int basicSubscriptionUsersCount = await _subscriptionService.CountBasicSubscribersAsync();



            int premiumSubscriptionUSersCount = await _subscriptionService.CountPremiumSubscribersAsync();

            // Creating a view model to pass the counts to the view
            var viewModel = new StatisticsViewModel
            {
                UserCount = userCount,
                ArticleCount = articleCount,
                BasicSubscriptionUsersCount = basicSubscriptionUsersCount,
                PremiumSubscriptionUsersCount = premiumSubscriptionUSersCount
            };

            return View(viewModel);
        }

       
    }

}
    

