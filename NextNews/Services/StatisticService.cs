using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextNews.Data;

namespace NextNews.Services
{
    public class StatisticService:IStatisticService
    {
        private readonly ApplicationDbContext _context;

        private readonly IUserService _userService;
        private readonly IArticleService _articleService;
        private readonly ISubscriptionService _subscriptionService;


     


        public StatisticService(IUserService userService, IArticleService articleService, ISubscriptionService subscriptionService)
        {
            _userService = userService;
            _articleService = articleService;
            _subscriptionService = subscriptionService;
        }

        public int GetUserCount()
        {
            return _userService.GetUsers().Count();
        }

        public int GetArticleCount()
        {
            return _articleService.GetArticles().Count();
        }



        public int GetBasicSubscrptionUsers()
        {
            return _context.Subscriptions.Count(subscription => subscription.SubscriptionType.Name == "Basic");
        }

       

        public int GetPremiumSubscriptionUsers()
        {
            return _context.Subscriptions.Count(subscription => subscription.SubscriptionType.Name == "Premium");
        }



    }
}
