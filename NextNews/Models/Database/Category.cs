using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace NextNews.Models.Database
{
    public class Category
    {
       
        public int Id { get; set; }

        [Display(Name = "Category Name : ")]
        public string? Name { get; set; }

        public string? PictureUrl { get; set; }

        public ICollection<Article>? Articles {  get; set; }    
        
    }
}
