using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NextNews.Models;
using NextNews.Models.Database;
using NextNews.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextNews.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp;
using Pager = NextNews.Models.Pager;
using Microsoft.AspNetCore.Routing;
using System;
using NextNews.Views.Shared.Components.SearchBar;
using NextNews.Data;
using Stripe;



namespace NextNews.Controllers
{
    // For Editor-specific actions
    //[Authorize(Policy = "Editor")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly ISubscriptionService _subscriptionService;



        public ArticleController(
            ApplicationDbContext context, 
            IArticleService articleService, 
            IUserService userService, 
            UserManager<User> userManager, 
            IWebHostEnvironment webHostEnvironment, 
            ISubscriptionService subscriptionService)
        {
            _articleService = articleService;
            _userService = userService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _subscriptionService = subscriptionService;
        }


        public IActionResult Index(string SearchText= "" ,  int pg =1)
        {
            List<Article> articles;
            if (SearchText != "" && SearchText != null)
            {
                articles = _context.Articles
                    .Where(p => p.HeadLine.Contains(SearchText))
                    .ToList();
            }
            else
                articles = _context.Articles.ToList();

            SPager SearchPager = new SPager()
            { Action="Index", Controller="Article", SearchText= SearchText};
            ViewBag.SearchText = SearchText;




            const int pageSize = 10;
            if (pg < 1)
                pg = 1;

            int recsCount = articles.Count();
            int recSkip = (pg - 1 ) * pageSize;
            List<Article> retArticles = articles.Skip(recSkip).Take(pageSize).ToList();
            SPager SearchPager2 = new SPager(recsCount, pg, pageSize) { Action = "Index", Controller = "Article", SearchText = "SearchText" };
            ViewBag.SearchPager = SearchPager2;
            return View(retArticles);
            //return View(articles);
            //return RedirectToAction(nameof(ListArticles));
        }


        // Latest articles
        public ActionResult LatestArticles()
        {
            var latestArticles = _articleService.GetArticles().OrderByDescending(obj => obj.DateStamp).Take(5).ToList();

            List<LatestNewsViewModel> vmList = new List<LatestNewsViewModel>();

            foreach (var item in latestArticles)
            {
                var vm = new LatestNewsViewModel()
                {
                    Id = item.Id,
                    HeadLine = item.HeadLine,
                    DateStamp = item.DateStamp,
                    ContentSummary = item.ContentSummary

                };

                vmList.Add(vm);
            }

            return View(vmList);
        }

       


        ////Action for list of article
        //public IActionResult ListArticles(int categoryId, string latestOrMostPopular, string editorsChoice, int pg = 1)
        //{
        //    var articles = _articleService.GetArticlesAndArchiveArticles();

        //    if (categoryId != 0)
        //    {
        //        articles = articles.Where(a => a.CategoryId == categoryId).ToList();
        //    }

        //    if(latestOrMostPopular == "latest")
        //    {
        //        articles = articles.OrderByDescending(a => a.DateStamp).ToList();
        //    }
        //    else if (latestOrMostPopular == "mostpopular")
        //    {
        //        articles = articles.OrderByDescending(a => a.Likes).ToList();
        //    }

        //    if (editorsChoice == "editorschoice")
        //    {
        //        articles = articles.Where(a => a.IsEditorsChoice == true).ToList();
        //    }


        //    const int pageSize = 9;
        //    if (pg < 1)
        //        pg = 1;
        //    int recsCount = articles.Count;
        //    var pager = new Pager(recsCount, pg, pageSize);
        //    int recSkip = (pg - 1) * pageSize;
        //    var data = articles.Skip(recSkip).Take(pager.PageSize).ToList();
        //    ViewBag.Pager = pager;
        //    return View(data);
        //    //return View(articles);
        //}
        // Action for list of articles
        public IActionResult ListArticles(int categoryId, string latestOrMostPopular, string editorsChoice, int pg = 1)
        {
            var articles = _articleService.GetArticlesAndArchiveArticles();

            if (categoryId != 0)
            {
                articles = articles.Where(a => a.CategoryId == categoryId).ToList();
            }

            if (latestOrMostPopular == "latest")
            {
                articles = articles.OrderByDescending(a => a.DateStamp).ToList();
            }
            else if (latestOrMostPopular == "mostpopular")
            {
                articles = articles.OrderByDescending(a => a.Likes).ToList();
            }

            if (editorsChoice == "editorschoice")
            {
                articles = articles.Where(a => a.IsEditorsChoice == true).ToList();
            }

            // Sort articles by Id in descending order
            articles = articles.OrderByDescending(a => a.Id).ToList();

            const int pageSize = 9;
            if (pg < 1)
                pg = 1;
            int recsCount = articles.Count;
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = articles.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);
        }



        public ActionResult LatestArticlesByCategory(int CategoryId)
        {
            var latestArticles = _articleService.GetArticles().OrderByDescending(obj => obj.DateStamp).Where(c => c.CategoryId == CategoryId).ToList();

            Dictionary<string, List<Article>> articlesByCategory = new Dictionary<string, List<Article>>();

            foreach (var article in latestArticles)
            {
                // Check if the article's category is not null
                if (article.Category != null)
                {
                    string categoryName = article.Category.Name;

                    if (!articlesByCategory.ContainsKey(categoryName))
                    {
                        articlesByCategory[categoryName] = new List<Article>();
                    }
                    articlesByCategory[categoryName].Add(article);
                }
            }

            return View(articlesByCategory);
        }


        public IActionResult LatestMostpopularEditorschoice(string latestOrMostPopular, string editorsChoice)
        {

            var articles = _articleService.GetArticlesAndArchiveArticles();

            if (!string.IsNullOrEmpty(latestOrMostPopular))
            {

                if (latestOrMostPopular == "latest")
                {
                    articles = articles.OrderByDescending(a => a.DateStamp).ToList();
                    ViewBag.Heading = "Latest articles";
                }
                else if (latestOrMostPopular == "mostpopular")
                {
                    articles = articles.OrderByDescending(a => a.Likes).ToList();
                    ViewBag.Heading = "Most popular articles";
                }

            }

            if (!string.IsNullOrEmpty(editorsChoice))
            {
                if (editorsChoice == "editorschoice")
                {
                    articles = articles.Where(a => a.IsEditorsChoice).ToList();
                    ViewBag.Heading = "Editors choice articles";

                }

            }

            return View(articles);
        }



        //Action to Add/Create article
        [HttpPost]
        [Authorize(Roles = "Editor")]
        public IActionResult AddArticle(Article article)
        {
            article.DateStamp = DateTime.Now;

            if (ModelState.IsValid)
            {
                article.ImageLink = _articleService.UploadImage(article.ImageFile).Result;

                _articleService.AddArticle(article);

                //article.ImageLink = _articleService.UploadImage(article.ImageFile).Result;
                return RedirectToAction("Listarticles");

                // Check if an image file is uploaded
                if (article.ImageFile != null && article.ImageFile.Length > 0)
                {
                    // Process the uploaded file, save it to the server, and set the ImageLink property
                    // This is just a basic example, you might want to implement more robust file handling
                    // For simplicity, I'm assuming you have an Images folder in your wwwroot directory
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + article.ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        article.ImageFile.CopyTo(fileStream);
                    }

                    article.ImageLink = "/Images/" + uniqueFileName; // Update the ImageLink property with the file path
                    article.ImageLink2 = "/Images/" + uniqueFileName;
                }

                //_articleService.AddArticle(article);

                //return RedirectToAction("ListArticles");
            }

            // If there is an error, repopulate the categories
            var categories = _articleService.GetCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View("CreateArticle", article);
        }



        [Authorize(Roles = "Editor")]
        public IActionResult CreateArticle()
        {
            var categories = _articleService.GetCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View();
        }


        //details
        //[Authorize(Roles = "Editor")]
        public async Task<IActionResult> Details(int id)
        {
            List<Article> allArticles = _articleService.GetArticlesAndArchiveArticles().ToList();
            var temp = allArticles.FirstOrDefault(a => a.Id == id);

            var vm = new ArticleDetailsViewModel()
            {
                Article = allArticles.FirstOrDefault(a => a.Id == id),
                LatestArticles = allArticles.Where(a=>a.Id !=id)
                .OrderByDescending(a => a.DateStamp).Take(3).ToList(),
            };
            _articleService.IncreamentViews(vm);



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










            return View(vm);

            //var article = await _articleService.GetArticleByIdAsync(id);

            //if (article == null)
            //{
            //    return NotFound();
            //}



            //return View(article);
        }



        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor")] // Only authorized users can edit categories
        public async Task<IActionResult> Edit(int id, Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _articleService.UpdateArticleAsync(article);
                return RedirectToAction(nameof(ListArticles));
            }

            return View(article);
        }


        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _articleService.DeleteArticleAsync(id);
            return RedirectToAction(nameof(ListArticles));
        }


        // Method to add likes
        [Authorize(Roles = "Premium")]
        public IActionResult Likes(int id, string returnUrl)
        {
            var userId = _userManager.GetUserId(User)!;
            _articleService.AddLikes(id, userId);

            return Json(new { success = true, message = "Action performed successfully" });

        }


        [Authorize(Roles = "Admin, Editor")]
        public IActionResult EditorDashboard()
        {
            return View();
        }


        // EditorsChoiceArticles
        public IActionResult EditorsChoice()
        {
            List<Article> editorsChoiceArticles = _articleService.GetArticles();
     
            return View(editorsChoiceArticles);
        }

        
        public IActionResult SaveEditorsChoice(string addOrRemove, int articleId)
        {
                _articleService.addOrRemoveEditorsChoice(addOrRemove, articleId);

            return RedirectToAction("EditorsChoice");
        }

        public IActionResult ArchiveArticle() 
        { 
         var articles=  _articleService.GetArchiveArticles();
            return View(articles);
        }



    }
}