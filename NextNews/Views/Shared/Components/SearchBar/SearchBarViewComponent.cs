using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace NextNews.Views.Shared.Components.SearchBar
{
    public class SearchBarViewComponent: ViewComponent
    {


        public  SearchBarViewComponent()
        {
        }

        public IViewComponentResult Invoke(SPager searchPager)
        {
            searchPager.Controller = "Categories";
            searchPager.Action = "Search";

            return View("Default", searchPager);
        }



    }


}
