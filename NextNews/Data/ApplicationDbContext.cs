using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NextNews.Models.Database;
using NextNews.ViewModels;
using NextNews.Models;


namespace NextNews.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

            
        }
    
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionType> SubscriptionTypes { get; set; }

        public DbSet<ContactFormMessage> ContactFormMessages { get; set; }


        public DbSet<NewsLetterSubscriber> NewsLetterSubscribers { get; set; }

    }
}