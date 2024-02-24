using Microsoft.AspNetCore.Mvc;
using NextNews.Data;

using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web.Mvc;
using NextNews.Models;




namespace NextNews.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }



        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
            {
                _context = context;
            }

            public ActionResult Index(string query, int page = 1, int perPage = 10)
            {
                var articles = _context.Articles
                    .Where(a => a.HeadLine.Contains(query) || a.Content.Contains(query))
                    .Skip((page -1) * perPage)
                    .Take(perPage)
                    .ToList();

                int totalCount = articles.Count(); 
                int totalPages = (int)Math.Ceiling((double)totalCount / perPage);
                var pagginatedArticles = articles.Skip((page - 1) * perPage).Take(perPage).ToList();

                ViewBag.Articles = pagginatedArticles;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View();
            }
        









    }
}






