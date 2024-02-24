using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NextNews.Data;
using NextNews.Models;
using NextNews.Models.Database;
using SQLitePCL;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Collections.Generic;
using Azure.Storage.Blobs;
using NextNews.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;


namespace NextNews.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;


        public ArticleService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _blobServiceClient = new BlobServiceClient(_configuration["AzureWebJobsStorage"]);
        }


        public List<Article> GetArticles()
        {
            var objList = _context.Articles.Include(x => x.UsersLiked).Include(c => c.Category)/*.Where(x=>x.Archive==false)*/.ToList();
            return objList;
        }

    //    // Method to get latest articles
    //public List<Article> GetLatestArticles()
    //{
    //    return GetArticles().OrderByDescending(obj => obj.DateStamp).ToList();
    //}

    //// Method to get most popular articles
    //public List<Article> GetMostPopularArticles()
    //{
    //    return GetArticles().OrderByDescending(obj => obj.Likes).ToList();
    //}

    //// Method to get editor's choice articles
    //public List<Article> GetEditorsChoiceArticles()
    //{
    //    return GetArticles().Where(a => a.IsEditorsChoice).ToList();
    //}






        public void AddArticle(Article article)
        {
            _context.Articles.Add(article);
            _context.SaveChanges();
            List<Category> categories = _context.Categories.ToList();
        }


        //details
        public async Task<Article> GetArticleByIdAsync(int id)
        {
            return await _context.Articles.FindAsync(id);
        }


        //Update 
        public async Task UpdateArticleAsync(Article article)
        {
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }


        //delete category
        public async Task DeleteArticleAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }
        }


        //Get Categories to select them in create view
        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }


        //Add no. of likes
        public void AddLikes(int articleId, string userId)
        {
            var article = _context.Articles.Include(x => x.UsersLiked).FirstOrDefault(x => x.Id == articleId);

            if (article is null) return;

            if (article.UsersLiked.Any(x => x.Id == userId))
            {
                article.UsersLiked = article.UsersLiked.Where(x => x.Id != userId).ToList();
                article.Likes = article.Likes + 1;
            }
            else
            {
                var user = _context.Users.Find(userId);
                if (user is null) return;

                article.UsersLiked.Add(user);
                article.Likes = article.Likes - 1;
            }

            _context.SaveChanges();

        }


        public void IncreamentViews(ArticleDetailsViewModel article)
        {
            if (article.Article.Views is null)
            {
                article.Article.Views = 1;
            }
            else
            {
                article.Article.Views++;

            }
            _context.SaveChanges();

        }


        public IEnumerable<Article> GetArticlesByCategory(int categoryId)
        {
            return _context.Articles.Where(a => a.CategoryId == categoryId).ToList();
        }


        public async Task<string> UploadImage(IFormFile file)
        {
            BlobContainerClient containerClient = _blobServiceClient
            .GetBlobContainerClient("articleimage");
            BlobClient blobClient = containerClient.GetBlobClient(file.FileName);
            await using (var stream = file.OpenReadStream())
            {
                blobClient.Upload(stream);
            }
            return blobClient.Uri.AbsoluteUri;
        }


        public List<Article> GetEditorsChoiceArticles()
        {
            var objList = _context.Articles.Where(a => a.IsEditorsChoice == true).ToList();

            return objList;
        }


        public void addOrRemoveEditorsChoice(string addOrRemove, int articleId)
        {
            Article obj = _context.Articles.Find(articleId);


            if (addOrRemove == "add")
            {
                obj.IsEditorsChoice = true;
                _context.Update(obj);
                _context.SaveChanges();
            }
            else if (addOrRemove == "remove")
            {
                obj.IsEditorsChoice = false;
                _context.Update(obj);
                _context.SaveChanges();
            }
        }


        public void CheckExpiredSubs()
        {
            var expiredSubscription = _context.Subscriptions.Where(s => s.Expired < DateTime.Now).ToList();
            foreach (var item in expiredSubscription)
            {
                item.IsActive = false;
                _context.Update(item);
            }
            _context.SaveChanges();
        }


        public async Task<List<LatestNewsViewModel>> LatestArticles()
        {
            var latestArticles = _context.Articles.OrderByDescending(obj => obj.DateStamp).Take(4).ToList();

            List<LatestNewsViewModel> vmList = new List<LatestNewsViewModel>();

            foreach (var item in latestArticles)
            {
                var vm = new LatestNewsViewModel()
                {
                    Id = item.Id,
                    HeadLine = item.HeadLine,
                    ImageLink = GetSmallImageLink(item.ImageLink),
                    ContentSummary = item.ContentSummary,
                    DateStamp = item.DateStamp,
                    ArticleUrl = GetDetailArticle(item.Id)

                };

                vmList.Add(vm);
            }

            return vmList;

        }


        private string GetSmallImageLink(string originalLink)
        {
            if (string.IsNullOrEmpty(originalLink))
            {
                return originalLink;
            }

            var baseSmallImageUrl = "https://nextnews.blob.core.windows.net/sample-images-sm/";
            var fileName = originalLink.Split('/').Last();

            return baseSmallImageUrl + fileName;
        }


        private string GetDetailArticle(int id)
        {

            return $"https://nextnews.azurewebsites.net/Article/Details/{id}";
        }


        public void ArticlesToArchive() 
        {
            var cutOffdate = DateTime.Now.AddDays(-30);
            var articlesToArchive = _context.Articles.Where(a=>a.DateStamp<cutOffdate && !a.Archive).ToList();
            foreach (var item in articlesToArchive)
            {
                item.Archive = true;
                _context.Update(item);
            }
            
            _context.SaveChanges();
           
        }


        public List<Article> GetArticlesAndArchiveArticles()
        {
            var objList = _context.Articles.Include(x => x.UsersLiked).ToList();
            return objList;
        }


        public int GetCategoryIdCategoryName(string categoryName)
        {
            var categoryId = _context.Categories.FirstOrDefault(c => c.Name == categoryName).Id;

            return categoryId;
        }

        public List<Article> GetArchiveArticles() 
        { 
        var articles =_context.Articles.Where(a => a.Archive==true).ToList();   
            return articles;
        } 



    }

}



