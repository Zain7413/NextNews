using System.ComponentModel.DataAnnotations;

namespace NextNews.Models.Database
{
    public class SubscriptionType
    {
        
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set;}

        public decimal Price { get; set; }

        public virtual ICollection<Subscription>? Subscriptions { get; set; }
    
    }
}
