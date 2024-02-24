using NextNews.Data;
using NextNews.Models.Database;

namespace NextNews.Services
{
    public class NewsLetterService : INewsLetterService
    {
        private readonly ApplicationDbContext _context;
        public NewsLetterService(ApplicationDbContext context) 
        { 
        _context = context;
        }
      
        public async Task CreateNLSubscriber(NewsLetterSubscriber newsLetterSubscriber)
        {
            _context.NewsLetterSubscribers.Add(newsLetterSubscriber);
            _context.SaveChanges();
        }
        public List<NewsLetterSubscriber> GetNewsLetterSubscribers() 
        { 
        var subscriberList=_context.NewsLetterSubscribers.ToList();
            return subscriberList;
        }
    }
}
