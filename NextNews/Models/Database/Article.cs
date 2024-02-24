using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace NextNews.Models.Database
{
    public class Article
    {
      
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateStamp { get; set; }

        public string? LinkText { get; set; }

        public string? HeadLine { get; set; }
        
        public string? ContentSummary { get; set; }

        public string? Content { get; set; }

        public bool IsEditorsChoice { get; set; } = false;

        public int? Views { get; set; }

        public int? Likes { get; set;}

        //public int? Dislike { get; set; }
        [Display(Name = "LinkImage1")]
        public string? ImageLink { get; set; }
        
        [Display(Name = "LinkImage2 (optional)")]
        public string? ImageLink2 { get; set; }

        [Display(Name = "Author:")]
        public string? AuthorName { get; set; }

        [ForeignKey("CategoryId")]
        public int? CategoryId {  get; set; }

        
        public virtual Category? Category { get; set; }
        public ICollection<User>? UsersLiked { get; set; }

        [NotMapped]   // not to saved in the table
        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }
        public bool Archive { get; set; }

    }
}
