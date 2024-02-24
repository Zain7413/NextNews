using NextNews.Models.Database;

namespace NextNews.ViewModels
{
    public class UserVM
    {
        public User User { get; set; }

        public List<Subscription>  MySubscriptions { get; set; }

    }
}
