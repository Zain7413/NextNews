using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueTrggerForNewLetter.Model
{
    public class NewsItemFM
    {
        public int Id { get; set; }
        public string? HeadLine { get; set; }
        public string? ContentSummary { get; set; }
        public DateTime? DateStamp { get; set; }
        public string? ImageLink { get; set; }
        public string ArticleUrl { get; set; }

    }
}
