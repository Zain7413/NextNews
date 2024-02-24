using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NextNews.Models.Database
{
    public class NewsLetterSubscriber
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [JsonIgnore]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

       
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [JsonIgnore]
        public DateTime? DateofBirth { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

    }
}
