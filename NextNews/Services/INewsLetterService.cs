using NextNews.Models.Database;

namespace NextNews.Services
{
    public interface INewsLetterService
    {
        public Task CreateNLSubscriber(NewsLetterSubscriber newsLetterSubscriber);
        public List<NewsLetterSubscriber> GetNewsLetterSubscribers();
    }
}
