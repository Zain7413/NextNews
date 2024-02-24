using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace NextNews.Models.Database;

public class User: IdentityUser
{




    [Display(Name = "First Name")]
    public string? FirstName { get; set; }


    [Display(Name = "Last Name")]
    public string? LastName { get; set; }


    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime? DateofBirth { get; set; }


    public ICollection<Subscription> Subscriptions { get; set; }

    public ICollection<Article> LikedArtilces { get; set; }





}
