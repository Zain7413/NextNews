using NextNews.Models.Database;

namespace NextNews.ViewModels
{
    public class ArticleDetailsViewModel
    {
        public Article Article { get; set; }
      
        public List<Article>? LatestArticles { get; set; }

    
    }
}
