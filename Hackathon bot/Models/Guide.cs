using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonBot.Models
{
    [Serializable]
    public class Guide : IArticle
    {
        public string Title { get; set; }
        public List<GuideSection> Sections { get; set; }
    }

    [Serializable]
    public class GuideSection
    {
        public string Title { get; set; }
        public List<Content> Contents { get; set; } // text for Text ContentType, path for image 
    }
}
