using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NextNews.Models.Database;
using NextNews.Services;
using NextNews.ViewModels;

namespace NextNews.Controllers
{
    //// For Admin-specific actions
    //[Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {

        private readonly IRoleManagementService _roleManagementService;
        private readonly IArticleService _articleService;
        private object _context;
        private readonly IUserService _userService;
        private readonly ISubscriptionService _subscriptionService;

        public AdminController(IRoleManagementService roleManagementService,IArticleService articleService ,IUserService userService,ISubscriptionService subscriptionService)
        {
            _roleManagementService = roleManagementService;
            _articleService = articleService;
            _userService = userService;
            _subscriptionService= subscriptionService;
            
        }


        [Authorize(Roles = "Admin, Editor")]
        public async Task <IActionResult> Index()
        {
            // Fetching user count and article count from services
            int userCount = _userService.GetUsers().Count();
            int articleCount = _articleService.GetArticles().Count();

            // Count basic subscription users asynchronously
            int basicSubscriptionUsersCount = await _subscriptionService.CountBasicSubscribersAsync();
            int premiumsubscriptionUsersCount = await _subscriptionService.CountPremiumSubscribersAsync();


            // Pass counts directly to the view
            ViewData["UserCount"] = userCount;
            ViewData["ArticleCount"] = articleCount;
            ViewData["BasicSubscriptionUsersCount"] = basicSubscriptionUsersCount;

            ViewData["PremiumSubscriptionUsersCount"] = premiumsubscriptionUsersCount;

            return View();
        }


        // This is for creating dynamic role wihout seeding

        /*
            [HttpGet]
            public IActionResult CreateRole()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> CreateRole(string roleName)
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                bool result = await _roleManagementService.CreateRoleAsync(roleName);

                if (result)
                {

                    return RedirectToAction("CreationSuccess", "Admin");
                }
                else
                {

                    return View("CreationFailed", "Admin");
                }
            }


            [HttpGet]
            public IActionResult CreationSuccess()
            {        
                return View();
            }


            [HttpGet]
            public IActionResult CreationFailed()
            {
                return View();
            }

    */

        [Authorize(Roles = "Admin, Editor")]
        public IActionResult Dashboard()
        {
            return View();
        }


    }
}
