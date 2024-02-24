using NextNews.Models.Database;

namespace NextNews.ViewModels
{
    public class NavbarCategoryVM
    {

        public List<Article> ArticlesInBoxes { get; set; }

        public List<Article> ArticlesInList { get; set; }

        public string CategoryName { get; set; }

    }
}
