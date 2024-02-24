using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueTrggerForNewLetter.Model
{
    public class NewsLetterFM
    {
        public string FirstName { get; set; } = "";
        public string Email { get; set; } = "";  
        public List<NewsItemFM> News { get; set; } = new List<NewsItemFM>();
       
    }
}
