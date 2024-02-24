using NextNews.Models.Database;

namespace NextNews.ViewModels
{
    public class NewsLetterVM
    {
        public NewsLetterSubscriber NewsLetterSubscriber { get; set; }
        public List <Article> News { get; set; }
    }
}
