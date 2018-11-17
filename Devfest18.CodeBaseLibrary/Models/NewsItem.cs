using System;

namespace Devfest18.CodeBaseLibrary.Models
{
    public class NewsArticle
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string ContentURL { get; set; }
        public string ThumbnailURL { get; set; }
        public DateTime PublishDate { get; set; }
    }
}